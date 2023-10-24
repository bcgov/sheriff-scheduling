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
    public class ShiftController : ControllerBase
    {
        public const string InvalidShiftError = "Invalid Shift.";
        public const string CannotUpdateCrossLocationError = "Cannot update cross location.";
        private ShiftService ShiftService { get; }
        private DutyRosterService DutyRosterService { get; }
        private CourtAdminDbContext Db { get; }
        private IConfiguration Configuration { get; }

        public ShiftController(ShiftService shiftService, DutyRosterService dutyRosterService, CourtAdminDbContext db, IConfiguration configuration)
        {
            ShiftService = shiftService;
            DutyRosterService = dutyRosterService;
            Db = db;
            Configuration = configuration;
        }

        #region Shift
        /// <summary>
        /// This is used in the main shift screen, also used in duty roster to populate the available courtAdmins on the right hand side. 
        /// </summary>
        [HttpGet]
        [PermissionClaimAuthorize(AuthorizeOperation.Or, Permission.ViewShifts, Permission.ViewDutyRoster)]
        public async Task<ActionResult<List<ShiftDto>>> GetShifts(int locationId, DateTimeOffset start, DateTimeOffset end, bool includeDuties = false)
        {
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationId)) return Forbid();
            if (!User.HasPermission(Permission.ViewDutyRoster)) includeDuties = false;

            var shifts = await ShiftService.GetShiftsForLocation(locationId, start, end, includeDuties);

            if (!User.HasPermission(Permission.ViewAllFutureShifts))
            {
                var location = await Db.Location.AsNoTracking().FirstOrDefaultAsync(l => l.Id == locationId);
                var timezone = location.Timezone;
                var restrictionDays = int.Parse(Configuration.GetNonEmptyValue("ViewShiftRestrictionDays"));
                var currentDate = DateTimeOffset.UtcNow.ConvertToTimezone(timezone).DateOnly();
                var endDate = currentDate.TranslateDateForDaylightSavings(timezone, restrictionDays + 1);
                shifts = shifts.WhereToList(s => s.StartDate < endDate);
            }

            return Ok(shifts.Adapt<List<ShiftDto>>());
        }

        [HttpPost]
        [PermissionClaimAuthorize(perm: Permission.CreateAndAssignShifts)]
        public async Task<ActionResult<List<ShiftDto>>> AddShifts(List<AddShiftDto> shiftDtos)
        {
            if (shiftDtos == null) return BadRequest(InvalidShiftError);
            var locationIds = shiftDtos.SelectDistinctToList(s => s.LocationId);
            if (locationIds.Count != 1) return BadRequest(CannotUpdateCrossLocationError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationIds.First())) return Forbid();

            var shift = await ShiftService.AddShifts(shiftDtos.Adapt<List<Shift>>());
            return Ok(shift.Adapt<List<ShiftDto>>());
        }

        [HttpPut]
        [PermissionClaimAuthorize(perm: Permission.EditShifts)]
        public async Task<ActionResult<List<ShiftDto>>> UpdateShifts(List<UpdateShiftDto> shiftDtos)
        {
            if (shiftDtos == null) return BadRequest(InvalidShiftError);
            var locationIds = await ShiftService.GetShiftsLocations(shiftDtos.SelectDistinctToList(s => s.Id));
            if (locationIds.Count != 1) return BadRequest(CannotUpdateCrossLocationError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationIds.First())) return Forbid();

            var shift = await ShiftService.UpdateShifts(DutyRosterService,shiftDtos.Adapt<List<Shift>>());
            return Ok(shift.Adapt<List<ShiftDto>>());
        }

        [HttpDelete]
        [PermissionClaimAuthorize(perm: Permission.ExpireShifts)]
        public async Task<ActionResult<ShiftDto>> ExpireShifts(List<int> ids)
        {
            var locationIds = await ShiftService.GetShiftsLocations(ids);
            if (locationIds.Count != 1) return BadRequest(CannotUpdateCrossLocationError);
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationIds.First())) return Forbid();

            await ShiftService.ExpireShiftsAndDutySlots(ids);
            return NoContent();
        }

        [HttpPost]
        [Route("importWeek")]
        [PermissionClaimAuthorize(perm: Permission.ImportShifts)]
        public async Task<ActionResult<ImportedShiftsDto>> ImportWeeklyShifts(int locationId, DateTimeOffset start)
        {
            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationId)) return Forbid();

            var shifts = await ShiftService.ImportWeeklyShifts(locationId, start);
            return Ok(shifts.Adapt<ImportedShiftsDto>());
        }
        #endregion

        /// <summary>
        /// This is used to show a worker's availability.
        /// </summary>
        [HttpGet]
        [Route("shiftAvailability")]
        [PermissionClaimAuthorize(perm: Permission.ViewShifts)]
        public async Task<ActionResult<List<ShiftAvailabilityDto>>> GetAvailability(int locationId, DateTimeOffset start, DateTimeOffset end)
        {
            if (start >= end) return BadRequest("Start date was on or after end date.");
            if (end.Subtract(start).TotalDays > 30) return BadRequest("End date and start date are more than 30 days apart.");

            if (!PermissionDataFiltersExtensions.HasAccessToLocation(User, Db, locationId)) return Forbid();

            var shiftAvailability = await ShiftService.GetShiftAvailability(start, end, locationId: locationId);

            if (!User.HasPermission(Permission.ViewAllFutureShifts))
            {
                var location = await Db.Location.AsNoTracking().FirstOrDefaultAsync(l => l.Id == locationId);
                var timezone = location.Timezone;
                var restrictionDays = int.Parse(Configuration.GetNonEmptyValue("ViewShiftRestrictionDays"));
                var currentDate = DateTimeOffset.UtcNow.ConvertToTimezone(timezone).DateOnly();
                var endDate = currentDate.TranslateDateForDaylightSavings(timezone, restrictionDays + 1);
                foreach (var sa in shiftAvailability)
                    sa.Conflicts = sa.Conflicts.WhereToList(c => c.Start < endDate);
            }

            return Ok(shiftAvailability.Adapt<List<ShiftAvailabilityDto>>());
        }
    }
}
