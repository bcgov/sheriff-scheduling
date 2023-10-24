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
    public class SheriffController : UserController
    {
        public const string CouldNotFindSheriffError = "Couldn't find sheriff.";
        public const string CouldNotFindSheriffEventError = "Couldn't find sheriff event.";
        private CourtAdminService SheriffService { get; }
        private ShiftService ShiftService { get; }
        private DutyRosterService DutyRosterService { get; }
        private CourtAdminDbContext Db { get; }

        // ReSharper disable once InconsistentNaming
        private readonly long _uploadPhotoSizeLimitKB;

        public SheriffController(CourtAdminService sheriffService, DutyRosterService dutyRosterService, ShiftService shiftService, UserService userUserService, IConfiguration configuration, CourtAdminDbContext db) : base(userUserService)
        {
            SheriffService = sheriffService;
            ShiftService = shiftService;
            DutyRosterService = dutyRosterService;
            Db = db;
            _uploadPhotoSizeLimitKB = Convert.ToInt32(configuration.GetNonEmptyValue("UploadPhotoSizeLimitKB"));
        }

        #region CourtAdmin

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateUsers)]
        public async Task<ActionResult<SheriffDto>> AddSheriff(SheriffWithIdirDto addSheriff)
        {
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, addSheriff.HomeLocationId)) return Forbid();

            var sheriff = addSheriff.Adapt<CourtAdmin>();
            sheriff = await SheriffService.AddCourtAdmin(sheriff);
            return Ok(sheriff.Adapt<SheriffDto>());
        }

        /// <summary>
        /// This gets a general list of Sheriffs. Includes Training, AwayLocation, Leave data within 7 days.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        public async Task<ActionResult<SheriffDto>> GetSheriffsForTeams()
        {
            var sheriffs = await SheriffService.GetFilteredCourtAdminsForTeams();
            return Ok(sheriffs.Adapt<List<SheriffDto>>());
        }

        /// <summary>
        /// This call includes all SheriffAwayLocation, SheriffLeave, SheriffTraining.
        /// </summary>
        /// <param name="id">Guid of the userid.</param>
        /// <returns>SheriffDto</returns>
        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        [Route("{id}")]
        public async Task<ActionResult<SheriffWithIdirDto>> GetSheriffForTeam(Guid id)
        {
            var sheriff = await SheriffService.GetFilteredCourtAdminForTeams(id);
            if (sheriff == null) return NotFound(CouldNotFindSheriffError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, sheriff.HomeLocationId)) return Forbid();

            var sheriffDto = sheriff.Adapt<SheriffWithIdirDto>();
            //Prevent exposing Idirs to regular users.
            sheriffDto.IdirName = User.HasPermission(Permission.EditIdir) ? sheriff.IdirName : null;
            return Ok(sheriffDto);
        }

        /// <summary>
        /// Development route, do not use this in application.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        [Route("self")]
        public async Task<ActionResult<SheriffDto>> GetSelfSheriff()
        {
            var sheriff = await SheriffService.GetFilteredCourtAdminForTeams(User.CurrentUserId());
            if (sheriff == null) return NotFound(CouldNotFindSheriffError);
            return Ok(sheriff.Adapt<SheriffDto>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffDto>> UpdateSheriff(SheriffWithIdirDto updateSheriff)
        {
            await CheckForAccessToSheriffByLocation(updateSheriff.Id);

            var canEditIdir = User.HasPermission(Permission.EditIdir);
            var sheriff = updateSheriff.Adapt<CourtAdmin>();
            sheriff = await SheriffService.UpdateCourtAdmin(sheriff, canEditIdir);

            return Ok(sheriff.Adapt<SheriffDto>());
        }

        [HttpPut]
        [Route("updateLocation")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffDto>> UpdateSheriffHomeLocation(Guid id, int locationId)
        {
            await CheckForAccessToSheriffByLocation(id);

            await SheriffService.UpdateSheriffHomeLocation(id, locationId);
            return NoContent();
        }

        [HttpGet]
        [Route("getPhoto/{id}")]
        [PermissionClaimAuthorize(perm: Permission.Login)]
        [ResponseCache(Duration = 15552000, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> GetPhoto(Guid id) => File(await SheriffService.GetPhoto(id), "image/jpeg");

        [HttpPost]
        [Route("uploadPhoto")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
        public async Task<ActionResult<SheriffDto>> UploadPhoto(Guid? id, string badgeNumber, IFormFile file)
        {
            await CheckForAccessToSheriffByLocation(id, badgeNumber);

            if (file.Length == 0) return BadRequest("File length = 0");
            if (file.Length >= _uploadPhotoSizeLimitKB * 1024) return BadRequest($"File length: {file.Length / 1024} KB, Maximum upload size: {_uploadPhotoSizeLimitKB} KB");

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var fileBytes = ms.ToArray();

            if (!fileBytes.IsImage()) return BadRequest("The uploaded file was not a valid GIF/JPEG/PNG.");

            var sheriff = await SheriffService.UpdateCourtAdminPhoto(id, badgeNumber, fileBytes);
            return Ok(sheriff.Adapt<SheriffDto>());
        }

        #endregion CourtAdmin

        #region CourtAdminAwayLocation

        [HttpPost]
        [Route("awayLocation")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffAwayLocationDto>> AddSheriffAwayLocation(SheriffAwayLocationDto sheriffAwayLocationDto, bool overrideConflicts = false)
        {
            await CheckForAccessToSheriffByLocation(sheriffAwayLocationDto.SheriffId);

            var sheriffAwayLocation = sheriffAwayLocationDto.Adapt<CourtAdminAwayLocation>();
            var createdSheriffAwayLocation = await SheriffService.AddSheriffAwayLocation(DutyRosterService, ShiftService, sheriffAwayLocation, overrideConflicts);
            return Ok(createdSheriffAwayLocation.Adapt<SheriffAwayLocationDto>());
        }

        [HttpPut]
        [Route("awayLocation")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffAwayLocationDto>> UpdateSheriffAwayLocation(SheriffAwayLocationDto sheriffAwayLocationDto, bool overrideConflicts = false)
        {
            await CheckForAccessToSheriffByLocation<CourtAdminAwayLocation>(sheriffAwayLocationDto.Id);

            var sheriffAwayLocation = sheriffAwayLocationDto.Adapt<CourtAdminAwayLocation>();
            var updatedSheriffAwayLocation = await SheriffService.UpdateSheriffAwayLocation(DutyRosterService, ShiftService, sheriffAwayLocation, overrideConflicts);
            return Ok(updatedSheriffAwayLocation.Adapt<SheriffAwayLocationDto>());
        }

        [HttpDelete]
        [Route("awayLocation")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult> RemoveSheriffAwayLocation(int id, string expiryReason)
        {
            await CheckForAccessToSheriffByLocation<CourtAdminAwayLocation>(id);

            await SheriffService.RemoveSheriffAwayLocation(id, expiryReason);
            return NoContent();
        }

        #endregion CourtAdminAwayLocation

        #region CourtAdminActingRank

        [HttpPost]
        [Route("actingRank")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffActingRankDto>> AddSheriffActingRank(SheriffActingRankDto sheriffActingRankDto, bool overrideConflicts = false)
        {
            var sheriffActingRank = sheriffActingRankDto.Adapt<CourtAdminActingRank>();
            var createdSheriffActingRank = await SheriffService.AddSheriffActingRank(DutyRosterService, ShiftService, sheriffActingRank, overrideConflicts);
            return Ok(createdSheriffActingRank.Adapt<SheriffActingRankDto>());
        }

        [HttpPut]
        [Route("actingRank")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffActingRankDto>> UpdateSheriffActingRank(SheriffActingRankDto sheriffActingRankDto, bool overrideConflicts = false)
        {
            var sheriffActingRank = sheriffActingRankDto.Adapt<CourtAdminActingRank>();
            var updatedSheriffActingRank = await SheriffService.UpdateSheriffActingRank(DutyRosterService, ShiftService, sheriffActingRank, overrideConflicts);
            return Ok(updatedSheriffActingRank.Adapt<SheriffActingRankDto>());
        }

        [HttpDelete]
        [Route("actingRank")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult> RemoveSheriffActingRank(int id, string expiryReason)
        {
            await SheriffService.RemoveSheriffActingRank(id, expiryReason);
            return NoContent();
        }

        #endregion CourtAdminActingRank

        #region CourtAdminLeave

        [HttpPost]
        [Route("leave")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffLeaveDto>> AddSheriffLeave(SheriffLeaveDto sheriffLeaveDto, bool overrideConflicts = false)
        {
            await CheckForAccessToSheriffByLocation(sheriffLeaveDto.SheriffId);

            var sheriffLeave = sheriffLeaveDto.Adapt<CourtAdminLeave>();
            var createdSheriffLeave = await SheriffService.AddSheriffLeave(DutyRosterService, ShiftService, sheriffLeave, overrideConflicts);
            return Ok(createdSheriffLeave.Adapt<SheriffLeaveDto>());
        }

        [HttpPut]
        [Route("leave")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffLeaveDto>> UpdateSheriffLeave(SheriffLeaveDto sheriffLeaveDto, bool overrideConflicts = false)
        {
            await CheckForAccessToSheriffByLocation<CourtAdminLeave>(sheriffLeaveDto.Id);

            var sheriffLeave = sheriffLeaveDto.Adapt<CourtAdminLeave>();
            var updatedSheriffLeave = await SheriffService.UpdateSheriffLeave(DutyRosterService, ShiftService, sheriffLeave, overrideConflicts);
            return Ok(updatedSheriffLeave.Adapt<SheriffLeaveDto>());
        }

        [HttpDelete]
        [Route("leave")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult> RemoveSheriffLeave(int id, string expiryReason)
        {
            await CheckForAccessToSheriffByLocation<CourtAdminLeave>(id);

            await SheriffService.RemoveSheriffLeave(id, expiryReason);
            return NoContent();
        }

        #endregion CourtAdminLeave

        #region CourtAdminTraining

        [HttpGet]
        [Route("training")]
        [PermissionClaimAuthorize(perm: Permission.GenerateReports)]
        public async Task<ActionResult<SheriffDto>> GetSheriffsTraining()
        {
            var sheriffs = await SheriffService.GetSheriffsTraining();
            return Ok(sheriffs.Adapt<List<SheriffDto>>());
        }

        [HttpPost]
        [Route("training")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffTrainingDto>> AddSheriffTraining(SheriffTrainingDto sheriffTrainingDto, bool overrideConflicts = false)
        {
            await CheckForAccessToSheriffByLocation(sheriffTrainingDto.SheriffId);

            var sheriffTraining = sheriffTrainingDto.Adapt<CourtAdminTraining>();
            var createdSheriffTraining = await SheriffService.AddSheriffTraining(DutyRosterService, ShiftService, sheriffTraining, overrideConflicts);
            return Ok(createdSheriffTraining.Adapt<SheriffTrainingDto>());
        }

        [HttpPut]
        [Route("training")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult<SheriffTrainingDto>> UpdateSheriffTraining(SheriffTrainingDto sheriffTrainingDto, bool overrideConflicts = false)
        {
            await CheckForAccessToSheriffByLocation<CourtAdminTraining>(sheriffTrainingDto.Id);

            var sheriffTraining = sheriffTrainingDto.Adapt<CourtAdminTraining>();
            if (!User.HasPermission(Permission.EditPastTraining))
            {
                var savedSheriffTraining = Db.SheriffTraining.AsNoTracking().FirstOrDefault(st => st.Id == sheriffTrainingDto.Id);
                if (savedSheriffTraining?.EndDate <= DateTimeOffset.UtcNow)
                    throw new BusinessLayerException("No permission to edit training that has completed.");
            }

            var updatedSheriffTraining = await SheriffService.UpdateSheriffTraining(DutyRosterService, ShiftService, sheriffTraining, overrideConflicts);
            return Ok(updatedSheriffTraining.Adapt<SheriffTrainingDto>());
        }

        [HttpDelete]
        [Route("training")]
        [PermissionClaimAuthorize(perm: Permission.EditUsers)]
        public async Task<ActionResult> RemoveSheriffTraining(int id, string expiryReason)
        {
            await CheckForAccessToSheriffByLocation<CourtAdminTraining>(id);

            if (!User.HasPermission(Permission.RemovePastTraining))
            {
                var sheriffTraining = Db.SheriffTraining.AsNoTracking().FirstOrDefault(st => st.Id == id);
                if (sheriffTraining?.EndDate <= DateTimeOffset.UtcNow)
                    throw new BusinessLayerException("No permission to remove training that has completed.");
            }

            await SheriffService.RemoveSheriffTraining(id, expiryReason);
            return NoContent();
        }

        #endregion CourtAdminTraining

        #region Access Helpers

        private async Task CheckForAccessToSheriffByLocation(Guid? id, string badgeNumber = null)
        {
            var savedSheriff = await SheriffService.GetCourtAdmin(id, badgeNumber);
            if (savedSheriff == null) throw new NotFoundException(CouldNotFindSheriffError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, savedSheriff.HomeLocationId)) throw new NotAuthorizedException();
        }

        private async Task CheckForAccessToSheriffByLocation<T>(int id) where T : CourtAdminEvent
        {
            var sheriffEvent = await SheriffService.GetSheriffEvent<T>(id);
            if (sheriffEvent == null) throw new NotFoundException(CouldNotFindSheriffEventError);
            var savedSheriff = await SheriffService.GetCourtAdmin(sheriffEvent.CourtAdminId, null);
            if (savedSheriff == null) throw new NotFoundException(CouldNotFindSheriffError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, savedSheriff.HomeLocationId)) throw new NotAuthorizedException();
        }

        #endregion Access Helpers
    }
}