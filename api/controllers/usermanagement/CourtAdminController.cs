using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CAS.API.helpers;
using CAS.API.helpers.extensions;
using CAS.API.infrastructure.authorization;
using CAS.API.infrastructure.exceptions;
using CAS.API.models.dto;
using CAS.API.models.dto.generated;
using CAS.API.services.scheduling;
using CAS.API.services.usermanagement;
using CAS.DB.models;
using CAS.DB.models.auth;
using CAS.DB.models.courtAdmin;

namespace CAS.API.controllers.usermanagement
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtAdminController : UserController
    {
        public const string CouldNotFindCourtAdminError = "Couldn't find courtAdmin.";
        public const string CouldNotFindCourtAdminEventError = "Couldn't find courtAdmin event.";
        private CourtAdminService CourtAdminService { get; }
        private ShiftService ShiftService { get; }
        private DutyRosterService DutyRosterService { get; }
        private CourtAdminDbContext Db { get; }

        // ReSharper disable once InconsistentNaming
        private readonly long _uploadPhotoSizeLimitKB;

        public CourtAdminController(CourtAdminService courtAdminService, DutyRosterService dutyRosterService, ShiftService shiftService, UserService userUserService, IConfiguration configuration, CourtAdminDbContext db) : base(userUserService)
        {
            CourtAdminService = courtAdminService;
            ShiftService = shiftService;
            DutyRosterService = dutyRosterService;
            Db = db;
            _uploadPhotoSizeLimitKB = Convert.ToInt32(configuration.GetNonEmptyValue("UploadPhotoSizeLimitKB"));
        }

        #region CourtAdmin

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateUsers)]
        public async Task<ActionResult<CourtAdminDto>> AddCourtAdmin(CourtAdminWithIdirDto addCourtAdmin)
        {
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, addCourtAdmin.HomeLocationId)) return Forbid();

            var courtAdmin = addCourtAdmin.Adapt<CourtAdmin>();
            courtAdmin = await CourtAdminService.AddCourtAdmin(courtAdmin);
            return Ok(courtAdmin.Adapt<CourtAdminDto>());
        }

        /// <summary>
        /// This gets a general list of CourtAdmins. Includes Training, AwayLocation, Leave data within 7 days.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        public async Task<ActionResult<CourtAdminDto>> GetCourtAdminsForTeams()
        {
            var courtAdmins = await CourtAdminService.GetFilteredCourtAdminsForTeams();
            return Ok(courtAdmins.Adapt<List<CourtAdminDto>>());
        }

        /// <summary>
        /// This call includes all CourtAdminAwayLocation, CourtAdminLeave, CourtAdminTraining.
        /// </summary>
        /// <param name="id">Guid of the userid.</param>
        /// <returns>CourtAdminDto</returns>
        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        [Route("{id}")]
        public async Task<ActionResult<CourtAdminWithIdirDto>> GetCourtAdminForTeam(Guid id)
        {
            var courtAdmin = await CourtAdminService.GetFilteredCourtAdminForTeams(id);
            if (courtAdmin == null) return NotFound(CouldNotFindCourtAdminError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, courtAdmin.HomeLocationId)) return Forbid();

            var courtAdminDto = courtAdmin.Adapt<CourtAdminWithIdirDto>();
            //Prevent exposing Idirs to regular users.
            courtAdminDto.IdirName = User.HasPermission(Permission.EditIdir) ? courtAdmin.IdirName : null;
            return Ok(courtAdminDto);
        }

        /// <summary>
        /// Development route, do not use this in application.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        [Route("self")]
        public async Task<ActionResult<CourtAdminDto>> GetSelfCourtAdmin()
        {
            var courtAdmin = await CourtAdminService.GetFilteredCourtAdminForTeams(User.CurrentUserId());
            if (courtAdmin == null) return NotFound(CouldNotFindCourtAdminError);
            return Ok(courtAdmin.Adapt<CourtAdminDto>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<CourtAdminDto>> UpdateCourtAdmin(CourtAdminWithIdirDto updateCourtAdmin)
        {
            await CheckForAccessToCourtAdminByLocation(updateCourtAdmin.Id);

            var canEditIdir = User.HasPermission(Permission.EditIdir);
            var courtAdmin = updateCourtAdmin.Adapt<CourtAdmin>();
            courtAdmin = await CourtAdminService.UpdateCourtAdmin(courtAdmin, canEditIdir);

            return Ok(courtAdmin.Adapt<CourtAdminDto>());
        }

        [HttpPut]
        [Route("updateLocation")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<CourtAdminDto>> UpdateCourtAdminHomeLocation(Guid id, int locationId)
        {
            await CheckForAccessToCourtAdminByLocation(id);

            await CourtAdminService.UpdateCourtAdminHomeLocation(id, locationId);
            return NoContent();
        }

        [HttpGet]
        [Route("getPhoto/{id}")]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        [ResponseCache(Duration = 15552000, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetPhoto(Guid id) => File(await CourtAdminService.GetPhoto(id), "image/jpeg");

        [HttpPost]
        [Route("uploadPhoto")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<ActionResult<CourtAdminDto>> UploadPhoto(Guid? id, string badgeNumber, IFormFile file)
        {
            await CheckForAccessToCourtAdminByLocation(id, badgeNumber);

            if (file.Length == 0) return BadRequest("File length = 0");
            if (file.Length >= _uploadPhotoSizeLimitKB * 1024) return BadRequest($"File length: {file.Length / 1024} KB, Maximum upload size: {_uploadPhotoSizeLimitKB} KB");

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();

            if (!fileBytes.IsImage()) return BadRequest("The uploaded file was not a valid GIF/JPEG/PNG.");

            var courtAdmin = await CourtAdminService.UpdateCourtAdminPhoto(id, badgeNumber, fileBytes);
            return Ok(courtAdmin.Adapt<CourtAdminDto>());
        }

        #endregion CourtAdmin

        #region CourtAdminAwayLocation

        [HttpPost]
        [Route("awayLocation")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<CourtAdminAwayLocationDto>> AddCourtAdminAwayLocation(CourtAdminAwayLocationDto courtAdminAwayLocationDto, bool overrideConflicts = false)
        {
            await CheckForAccessToCourtAdminByLocation(courtAdminAwayLocationDto.CourtAdminId);

            var courtAdminAwayLocation = courtAdminAwayLocationDto.Adapt<CourtAdminAwayLocation>();
            var createdCourtAdminAwayLocation = await CourtAdminService.AddCourtAdminAwayLocation(DutyRosterService, ShiftService, courtAdminAwayLocation, overrideConflicts);
            return Ok(createdCourtAdminAwayLocation.Adapt<CourtAdminAwayLocationDto>());
        }

        [HttpPut]
        [Route("awayLocation")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<CourtAdminAwayLocationDto>> UpdateCourtAdminAwayLocation(CourtAdminAwayLocationDto courtAdminAwayLocationDto, bool overrideConflicts = false)
        {
            await CheckForAccessToCourtAdminByLocation<CourtAdminAwayLocation>(courtAdminAwayLocationDto.Id);

            var courtAdminAwayLocation = courtAdminAwayLocationDto.Adapt<CourtAdminAwayLocation>();
            var updatedCourtAdminAwayLocation = await CourtAdminService.UpdateCourtAdminAwayLocation(DutyRosterService, ShiftService, courtAdminAwayLocation, overrideConflicts);
            return Ok(updatedCourtAdminAwayLocation.Adapt<CourtAdminAwayLocationDto>());
        }

        [HttpDelete]
        [Route("awayLocation")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult> RemoveCourtAdminAwayLocation(int id, string expiryReason)
        {
            await CheckForAccessToCourtAdminByLocation<CourtAdminAwayLocation>(id);

            await CourtAdminService.RemoveCourtAdminAwayLocation(id, expiryReason);
            return NoContent();
        }

        #endregion CourtAdminAwayLocation

        #region CourtAdminActingRank

        [HttpPost]
        [Route("actingRank")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<CourtAdminActingRankDto>> AddCourtAdminActingRank(CourtAdminActingRankDto courtAdminActingRankDto, bool overrideConflicts = false)
        {
            var courtAdminActingRank = courtAdminActingRankDto.Adapt<CourtAdminActingRank>();
            var createdCourtAdminActingRank = await CourtAdminService.AddCourtAdminActingRank(DutyRosterService, ShiftService, courtAdminActingRank, overrideConflicts);
            return Ok(createdCourtAdminActingRank.Adapt<CourtAdminActingRankDto>());
        }

        [HttpPut]
        [Route("actingRank")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<CourtAdminActingRankDto>> UpdateCourtAdminActingRank(CourtAdminActingRankDto courtAdminActingRankDto, bool overrideConflicts = false)
        {
            var courtAdminActingRank = courtAdminActingRankDto.Adapt<CourtAdminActingRank>();
            var updatedCourtAdminActingRank = await CourtAdminService.UpdateCourtAdminActingRank(DutyRosterService, ShiftService, courtAdminActingRank, overrideConflicts);
            return Ok(updatedCourtAdminActingRank.Adapt<CourtAdminActingRankDto>());
        }

        [HttpDelete]
        [Route("actingRank")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult> RemoveCourtAdminActingRank(int id, string expiryReason)
        {
            await CourtAdminService.RemoveCourtAdminActingRank(id, expiryReason);
            return NoContent();
        }

        #endregion CourtAdminActingRank

        #region CourtAdminLeave

        [HttpPost]
        [Route("leave")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<CourtAdminLeaveDto>> AddCourtAdminLeave(CourtAdminLeaveDto courtAdminLeaveDto, bool overrideConflicts = false)
        {
            await CheckForAccessToCourtAdminByLocation(courtAdminLeaveDto.CourtAdminId);

            var courtAdminLeave = courtAdminLeaveDto.Adapt<CourtAdminLeave>();
            var createdCourtAdminLeave = await CourtAdminService.AddCourtAdminLeave(DutyRosterService, ShiftService, courtAdminLeave, overrideConflicts);
            return Ok(createdCourtAdminLeave.Adapt<CourtAdminLeaveDto>());
        }

        [HttpPut]
        [Route("leave")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<CourtAdminLeaveDto>> UpdateCourtAdminLeave(CourtAdminLeaveDto courtAdminLeaveDto, bool overrideConflicts = false)
        {
            await CheckForAccessToCourtAdminByLocation<CourtAdminLeave>(courtAdminLeaveDto.Id);

            var courtAdminLeave = courtAdminLeaveDto.Adapt<CourtAdminLeave>();
            var updatedCourtAdminLeave = await CourtAdminService.UpdateCourtAdminLeave(DutyRosterService, ShiftService, courtAdminLeave, overrideConflicts);
            return Ok(updatedCourtAdminLeave.Adapt<CourtAdminLeaveDto>());
        }

        [HttpDelete]
        [Route("leave")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult> RemoveCourtAdminLeave(int id, string expiryReason)
        {
            await CheckForAccessToCourtAdminByLocation<CourtAdminLeave>(id);

            await CourtAdminService.RemoveCourtAdminLeave(id, expiryReason);
            return NoContent();
        }

        #endregion CourtAdminLeave

        #region CourtAdminTraining

        [HttpGet]
        [Route("training")]
        [PermissionClaimAuthorize(perm: Permission.GenerateReports)]
        public async Task<ActionResult<CourtAdminDto>> GetCourtAdminsTraining()
        {
            var courtAdmins = await CourtAdminService.GetCourtAdminsTraining();
            return Ok(courtAdmins.Adapt<List<CourtAdminDto>>());
        }

        [HttpPost]
        [Route("training")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<CourtAdminTrainingDto>> AddCourtAdminTraining(CourtAdminTrainingDto courtAdminTrainingDto, bool overrideConflicts = false)
        {
            await CheckForAccessToCourtAdminByLocation(courtAdminTrainingDto.CourtAdminId);

            var courtAdminTraining = courtAdminTrainingDto.Adapt<CourtAdminTraining>();
            var createdCourtAdminTraining = await CourtAdminService.AddCourtAdminTraining(DutyRosterService, ShiftService, courtAdminTraining, overrideConflicts);
            return Ok(createdCourtAdminTraining.Adapt<CourtAdminTrainingDto>());
        }

        [HttpPut]
        [Route("training")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<CourtAdminTrainingDto>> UpdateCourtAdminTraining(CourtAdminTrainingDto courtAdminTrainingDto, bool overrideConflicts = false)
        {
            await CheckForAccessToCourtAdminByLocation<CourtAdminTraining>(courtAdminTrainingDto.Id);

            var courtAdminTraining = courtAdminTrainingDto.Adapt<CourtAdminTraining>();
            if (!User.HasPermission(Permission.EditPastTraining))
            {
                var savedCourtAdminTraining = Db.CourtAdminTraining.AsNoTracking().FirstOrDefault(st => st.Id == courtAdminTrainingDto.Id);
                if (savedCourtAdminTraining?.EndDate <= DateTimeOffset.UtcNow)
                    throw new BusinessLayerException("No permission to edit training that has completed.");
            }

            var updatedCourtAdminTraining = await CourtAdminService.UpdateCourtAdminTraining(DutyRosterService, ShiftService, courtAdminTraining, overrideConflicts);
            return Ok(updatedCourtAdminTraining.Adapt<CourtAdminTrainingDto>());
        }

        [HttpDelete]
        [Route("training")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult> RemoveCourtAdminTraining(int id, string expiryReason)
        {
            await CheckForAccessToCourtAdminByLocation<CourtAdminTraining>(id);

            if (!User.HasPermission(Permission.RemovePastTraining))
            {
                var courtAdminTraining = Db.CourtAdminTraining.AsNoTracking().FirstOrDefault(st => st.Id == id);
                if (courtAdminTraining?.EndDate <= DateTimeOffset.UtcNow)
                    throw new BusinessLayerException("No permission to remove training that has completed.");
            }

            await CourtAdminService.RemoveCourtAdminTraining(id, expiryReason);
            return NoContent();
        }

        #endregion CourtAdminTraining

        #region Access Helpers

        private async Task CheckForAccessToCourtAdminByLocation(Guid? id, string badgeNumber = null)
        {
            var savedCourtAdmin = await CourtAdminService.GetCourtAdmin(id, badgeNumber);
            if (savedCourtAdmin == null) throw new NotFoundException(CouldNotFindCourtAdminError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, savedCourtAdmin.HomeLocationId)) throw new NotAuthorizedException();
        }

        private async Task CheckForAccessToCourtAdminByLocation<T>(int id) where T : CourtAdminEvent
        {
            var courtAdminEvent = await CourtAdminService.GetCourtAdminEvent<T>(id);
            if (courtAdminEvent == null) throw new NotFoundException(CouldNotFindCourtAdminEventError);
            var savedCourtAdmin = await CourtAdminService.GetCourtAdmin(courtAdminEvent.CourtAdminId, null);
            if (savedCourtAdmin == null) throw new NotFoundException(CouldNotFindCourtAdminError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, savedCourtAdmin.HomeLocationId)) throw new NotAuthorizedException();
        }

        #endregion Access Helpers
    }
}