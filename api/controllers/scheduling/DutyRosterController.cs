using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CAS.API.helpers;
using CAS.API.helpers.extensions;
using CAS.API.infrastructure.authorization;
using CAS.API.models.dto.generated;
using CAS.API.services.scheduling;
using CAS.COMMON.helpers.extensions;
using CAS.DB.models;
using CAS.DB.models.auth;
using CAS.DB.models.scheduling;

namespace CAS.API.controllers.scheduling
{
    [Route("api/[controller]")]
    [ApiController]
    public class DutyRosterController : ControllerBase
    {
        public const string InvalidDutyErrorMessage = "Invalid Duty.";
        public const string CannotUpdateCrossLocationError = "Cannot update cross location.";
        private DutyRosterService DutyRosterService { get; }
        private CourtAdminDbContext Db { get; }
        private IConfiguration Configuration { get; }
        public DutyRosterController(DutyRosterService dutyRosterService, CourtAdminDbContext db, IConfiguration configuration)
        {
            DutyRosterService = dutyRosterService;
            Db = db;
            Configuration = configuration;
        }

        /// <summary>
        /// This is for the center of the DutyRoster screen. Specifically when assignments are created into Duties. 
        /// </summary>
        [HttpGet]
        [PermissionClaimAuthorize(perm: Permission.ViewDutyRoster)]
        public async Task<ActionResult<List<DutyDto>>> GetDuties(int locationId, DateTimeOffset start, DateTimeOffset end)
        {
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationId)) return Forbid();
            var duties = await DutyRosterService.GetDutiesForLocation(locationId, start, end);

            if (!User.HasPermission(Permission.ViewDutyRosterInFuture))
            {
                var location = await Db.Location.AsNoTracking().FirstOrDefaultAsync(l => l.Id == locationId);
                var timezone = location.Timezone;
                var currentDate = DateTimeOffset.UtcNow.ConvertToTimezone(timezone).DateOnly();
                var restrictionHours = float.Parse(Configuration.GetNonEmptyValue("ViewDutyRosterRestrictionHours"));
                var endDate = currentDate.TranslateDateForDaylightSavings(timezone, hoursToShift: restrictionHours);
                duties = duties.WhereToList(d => d.StartDate < endDate);
            }

            return Ok(duties.Adapt<List<DutyDto>>());
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignDuties)]
        public async Task<ActionResult<List<DutyDto>>> AddDuties(List<AddDutyDto> newDuties)
        {
            if (newDuties == null) return BadRequest(InvalidDutyErrorMessage);
            var locationIds = newDuties.SelectDistinctToList(d => d.LocationId);
            if (locationIds.Count != 1) return BadRequest(CannotUpdateCrossLocationError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationIds.First())) return Forbid();

            var duty = await DutyRosterService.AddDuties(newDuties.Adapt<List<Duty>>());
            return Ok(duty.Adapt<List<DutyDto>>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditDuties)]
        public async Task<ActionResult<List<DutyDto>>> UpdateDuties(List<UpdateDutyDto> editDuties, bool overrideValidation = false)
        {
            if (editDuties == null) return BadRequest(InvalidDutyErrorMessage);
            var locationIds = await DutyRosterService.GetDutiesLocations(editDuties.SelectToList(d => d.Id));
            if (locationIds.Count != 1) return BadRequest(CannotUpdateCrossLocationError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationIds.First())) return Forbid();

            var duties = await DutyRosterService.UpdateDuties(editDuties.Adapt<List<Duty>>(), overrideValidation);
            return Ok(duties.Adapt<List<DutyDto>>());
        }

        [HttpPut("updateComment")]
        [PermissionClaimAuthorize(perm: Permission.EditDuties)]
        public async Task<ActionResult> UpdateDutyComment(int dutyId, string comment)
        {
            var locationIds = await DutyRosterService.GetDutiesLocations(new List<int> {dutyId});
            if (locationIds.Count != 1) return BadRequest(CannotUpdateCrossLocationError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationIds.First())) return Forbid();

            await DutyRosterService.UpdateDutyComment(dutyId, comment);
            return NoContent();
        }

        [HttpPut("moveCourtAdmin")]
        [PermissionClaimAuthorize(perm: Permission.EditDuties)]
        public async Task<ActionResult<DutyDto>> MoveCourtAdminFromDutySlot(int fromDutySlotId, int toDutyId, DateTimeOffset? separationTime = null)
        {
            var duty = await DutyRosterService.GetDutyByDutySlot(fromDutySlotId);
            if (duty == null) return NotFound();
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, duty.LocationId)) return Forbid();

            duty = await DutyRosterService.MoveCourtAdminFromDutySlot(fromDutySlotId, toDutyId, separationTime);
            return Ok(duty.Adapt<DutyDto>());
        }

        [HttpDelete]
        [PermissionClaimAuthorize(perm: Permission.ExpireDuties)]
        public async Task<ActionResult> ExpireDuties(List<int> ids)
        {
            if (ids == null) return BadRequest(InvalidDutyErrorMessage);
            var locationIds = await DutyRosterService.GetDutiesLocations(ids);
            if (locationIds.Count != 1) return BadRequest(CannotUpdateCrossLocationError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationIds.First())) return Forbid();

            await DutyRosterService.ExpireDuties(ids);
            return NoContent();
        }
    }
}
