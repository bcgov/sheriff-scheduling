using Microsoft.EntityFrameworkCore;
using CAS.API.helpers.extensions;
using CAS.API.infrastructure.exceptions;
using CAS.API.Models.DB;
using CAS.API.services.usermanagement;
using CAS.COMMON.helpers.extensions;
using CAS.DB.models;
using CAS.DB.models.scheduling.notmapped;
using CAS.DB.models.courtAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.Extensions.Configuration;
using CAS.API.helpers;
using CAS.API.models;
using CAS.DB.models.lookupcodes;
using Shift = CAS.DB.models.scheduling.Shift;

namespace CAS.API.services.scheduling
{
    public class ShiftService
    {
        private CourtAdminDbContext Db { get; }
        private CourtAdminService CourtAdminService { get; }
        public double OvertimeHoursPerDay { get; }

        public ShiftService(CourtAdminDbContext db, CourtAdminService courtAdminService, IConfiguration configuration)
        {
            Db = db;
            CourtAdminService = courtAdminService;
            OvertimeHoursPerDay = double.Parse(configuration.GetNonEmptyValue("OvertimeHoursPerDay"));
        }

        public async Task<List<Shift>> GetShiftsForLocation(int locationId, DateTimeOffset start, DateTimeOffset end, bool includeDuties)
        {
            var lookupCode = await Db.LookupCode.AsNoTracking()
                .Where(lc => lc.Type == LookupTypes.CourtAdminRank)
                .Include(s => s.SortOrder)
                .ToListAsync();

            var rankStartDate = DateTimeOffset.UtcNow;

            var shifts = await Db.Shift.AsSingleQuery().AsNoTracking()
                .Include(s => s.Location)
                .Include(s => s.CourtAdmin)
                .ThenInclude(s => s.ActingRank.Where(ar =>
                    (ar.StartDate <= rankStartDate && rankStartDate < ar.EndDate)
                    && ar.ExpiryDate == null))
                .Include(s => s.AnticipatedAssignment)
                .Where(s => s.LocationId == locationId && s.ExpiryDate == null &&
                            s.StartDate < end && start < s.EndDate)
                .ToListAsync();

            var dutySlots = await Db.DutySlot.AsSingleQuery()
                .AsNoTracking()
                .Include(ds => ds.Duty)
                .ThenInclude(d => d.Assignment)
                .ThenInclude(a => a.LookupCode)
                .Where(ds =>
                    includeDuties &&
                    ds.LocationId == locationId &&
                    ds.ExpiryDate == null &&
                    ds.StartDate < end &&
                    start < ds.EndDate)
                .ToListAsync();

            foreach (var shift in shifts)
                shift.DutySlots = dutySlots.Where(ds =>
                        ds.CourtAdminId == shift.CourtAdminId && ds.StartDate < shift.EndDate &&
                        shift.StartDate < ds.EndDate)
                    .ToList();

            return shifts.OrderBy(s => lookupCode.FirstOrDefault(so => so.Code == s.CourtAdmin?.Rank)
                ?.SortOrder.FirstOrDefault()
                ?.SortOrder)
            .ThenBy(s => s.CourtAdmin.LastName)
            .ThenBy(s => s.CourtAdmin.FirstName)
            .ToList();
        }

        public async Task<List<int>> GetShiftsLocations(List<int> ids) =>
            await Db.Shift.AsNoTracking().In(ids, s => s.Id).Select(s => s.LocationId).Distinct().ToListAsync();

        public async Task<List<Shift>> AddShifts(List<Shift> shifts)
        {
            var overlaps = await GetShiftConflicts(shifts);
            if (overlaps.Any()) throw new BusinessLayerException(overlaps.SelectMany(ol => ol.ConflictMessages).ToStringWithPipes());

            if (shifts.Any(s => s.StartDate >= s.EndDate))
                throw new BusinessLayerException($"{nameof(Shift)} Start date cannot come after end date.");

            if (shifts.Any(s => s.Timezone.GetTimezone() == null))
                throw new BusinessLayerException($"A valid {nameof(Shift.Timezone)} needs to be included in the {nameof(Shift)}.");

            shifts = SplitLongShifts(shifts);

            foreach (var shift in shifts)
                await AddShift(shift);

            await Db.SaveChangesAsync();

            await CalculateOvertimeHoursForShifts(shifts);

            return shifts;
        }

        public async Task<List<Shift>> UpdateShifts(DutyRosterService dutyRosterService, List<Shift> shifts)
        {
            var overlaps = await GetShiftConflicts(shifts);
            if (overlaps.Any()) throw new BusinessLayerException(overlaps.SelectMany(ol => ol.ConflictMessages).ToStringWithPipes());

            var shiftIds = shifts.SelectToList(s => s.Id);
            var savedShifts = Db.Shift.In(shiftIds, s => s.Id);

            if (shifts.Any(s => s.StartDate >= s.EndDate))
                throw new BusinessLayerException($"{nameof(Shift)} Start date cannot come after end date.");

            if (shifts.Any(s => s.Timezone.GetTimezone() == null))
                throw new BusinessLayerException($"A valid {nameof(Shift.Timezone)} needs to be included in the {nameof(Shift)}.");

            shifts = SplitLongShifts(shifts);

            foreach (var shift in shifts)
            {
                //Need to add shifts, because some of the shifts were split.
                if (shift.Id == 0)
                {
                    await AddShift(shift);
                    continue;
                }

                var savedShift = savedShifts.FirstOrDefault(s => s.Id == shift.Id);
                savedShift.ThrowBusinessExceptionIfNull($"{nameof(Shift)} with the id: {shift.Id} could not be found.");
                Db.Entry(savedShift!).CurrentValues.SetValues(shift);
                Db.Entry(savedShift).Property(x => x.LocationId).IsModified = false;
                Db.Entry(savedShift).Property(x => x.ExpiryDate).IsModified = false;

                savedShift.CourtAdmin = await Db.CourtAdmin.FindAsync(shift.CourtAdminId);
                savedShift.AnticipatedAssignment = await Db.Assignment.FindAsync(shift.AnticipatedAssignmentId);
            }
            await Db.SaveChangesAsync();

            await CalculateOvertimeHoursForShifts(shifts);

            await dutyRosterService.AdjustDutySlots(shifts);

            return await savedShifts.ToListAsync();
        }

        public async Task ExpireShiftsAndDutySlots(List<int> ids)
        {
            var removedShifts = await Db.Shift.In(ids, s => s.Id).ToListAsync();
            foreach (var shift in removedShifts)
            {
                shift!.ExpiryDate = DateTimeOffset.UtcNow;
                var dutySlots = Db.DutySlot.Where(d =>
                    d.ExpiryDate == null &&
                    d.CourtAdminId == shift.CourtAdminId &&
                    shift.StartDate <= d.StartDate &&
                    shift.EndDate >= d.EndDate);
                await dutySlots.ForEachAsync(ds =>
                {
                    ds.ExpiryDate = DateTimeOffset.UtcNow;
                });
            }

            await Db.SaveChangesAsync();

            await CalculateOvertimeHoursForShifts(removedShifts);

            await Db.SaveChangesAsync();
        }

        public async Task<ImportedShifts> ImportWeeklyShifts(int locationId, DateTimeOffset start)
        {
            var location = Db.Location.FirstOrDefault(l => l.Id == locationId);
            location.ThrowBusinessExceptionIfNull($"Couldn't find {nameof(Location)} with id: {locationId}.");
            var timezone = location?.Timezone;
            timezone.GetTimezone().ThrowBusinessExceptionIfNull("Timezone was invalid.");

            //We need to adjust to their start of the week, because it can differ depending on the TZ!
            var targetStartDate = start.ConvertToTimezone(timezone);
            var targetEndDate = targetStartDate.TranslateDateForDaylightSavings(timezone, 7);

            var courtAdminsAvailableAtLocation = await CourtAdminService.GetCourtAdminsForShiftAvailabilityForLocation(locationId, targetStartDate, targetEndDate);
            var courtAdminIds = courtAdminsAvailableAtLocation.SelectDistinctToList(s => s.Id);

            var shiftsToImport = Db.Shift
                .Include(s => s.Location)
                .Include(s => s.CourtAdmin)
                .AsNoTracking()
                .In(courtAdminIds, s => s.CourtAdminId)
                .Where(s => s.LocationId == locationId &&
                            s.ExpiryDate == null &&
                            s.StartDate < targetEndDate && targetStartDate < s.EndDate
                           );

            var importedShifts = await shiftsToImport.Select(shift => Db.DetachedClone(shift)).ToListAsync();
            foreach (var shift in importedShifts)
            {
                shift.StartDate = shift.StartDate.TranslateDateForDaylightSavings(timezone, 7);
                shift.EndDate = shift.EndDate.TranslateDateForDaylightSavings(timezone, 7);
            }

            var overlaps = await GetShiftConflicts(importedShifts);
            var filteredImportedShifts = importedShifts.WhereToList(s => overlaps.All(o => o.Shift.Id != s.Id) &&
                                                                         !overlaps.Any(ts =>
                                                                             s.Id != ts.Shift.Id && ts.Shift.StartDate < s.EndDate && s.StartDate < ts.Shift.EndDate &&
                                                                             ts.Shift.CourtAdminId == s.CourtAdminId));

            filteredImportedShifts.ForEach(s => s.Id = 0);
            await Db.Shift.AddRangeAsync(filteredImportedShifts);
            await Db.SaveChangesAsync();

            return new ImportedShifts
            {
                ConflictMessages = overlaps.SelectMany(o => o.ConflictMessages).ToList(),
                Shifts = filteredImportedShifts
            };
        }

        /// <summary>
        /// This is used for Distribute Schedule, as well as the Shift Schedule page.
        /// </summary>
        public async Task<List<ShiftAvailability>> GetShiftAvailability(DateTimeOffset start, DateTimeOffset end, int locationId)
        {
            var courtAdmins = await CourtAdminService.GetCourtAdminsForShiftAvailabilityForLocation(locationId, start, end);

            //Include courtAdmins that have a shift, but their home location / away location doesn't match.
            //Grey out on the GUI if HomeLocationId and AwayLocation doesn't match.
            var courtAdminIdsFromShifts = await Db.Shift.AsNoTracking()
                .Where(s => s.StartDate < end && start < s.EndDate && s.ExpiryDate == null &&
                            s.LocationId == locationId)
                .Select(s => s.CourtAdminId)
                .ToListAsync();

            var courtAdminsOutOfLocationWithShiftIds = courtAdminIdsFromShifts.Except(courtAdmins.Select(s => s.Id));

            //Note their AwayLocation, Leave, Training should be entirely empty, this is intentional.
            var courtAdminsOutOfLocationWithShift = await
               Db.CourtAdmin.AsNoTracking()
                   .Include(s => s.HomeLocation)
                   .In(courtAdminsOutOfLocationWithShiftIds,
                       s => s.Id)
                   .Where(s => s.IsEnabled)
                   .ToListAsync();

            courtAdmins = courtAdmins.Concat(courtAdminsOutOfLocationWithShift).ToList();

            var shiftsForCourtAdmins = await GetShiftsForCourtAdmins(courtAdmins.Select(s => s.Id), start, end);

            var courtAdminEventConflicts = new List<ShiftAvailabilityConflict>();
            courtAdmins.ForEach(courtAdmin =>
            {
                courtAdminEventConflicts.AddRange(courtAdmin.AwayLocation.Select(s => new ShiftAvailabilityConflict
                {
                    Conflict = ShiftConflictType.AwayLocation,
                    CourtAdminId = courtAdmin.Id,
                    Start = s.StartDate,
                    End = s.EndDate,
                    LocationId = s.LocationId,
                    Location = s.Location,
                    Timezone = s.Timezone,
                    Comment = s.Comment
                }));
                courtAdminEventConflicts.AddRange(courtAdmin.Leave.Select(s => new ShiftAvailabilityConflict
                {
                    Conflict = ShiftConflictType.Leave,
                    CourtAdminId = courtAdmin.Id,
                    Start = s.StartDate,
                    End = s.EndDate,
                    Timezone = s.Timezone,
                    CourtAdminEventType = s.LeaveType?.Code,
                    Comment = s.Comment
                }));
                courtAdminEventConflicts.AddRange(courtAdmin.Training.Select(s => new ShiftAvailabilityConflict
                {
                    Conflict = ShiftConflictType.Training,
                    CourtAdminId = courtAdmin.Id,
                    Start = s.StartDate,
                    End = s.EndDate,
                    Timezone = s.Timezone,
                    CourtAdminEventType = s.TrainingType?.Code,
                    Comment = s.Note
                }));
            });

            var existingShiftConflicts = shiftsForCourtAdmins.Select(s => new ShiftAvailabilityConflict
            {
                Conflict = ShiftConflictType.Scheduled,
                CourtAdminId = s.CourtAdminId,
                Location = s.Location,
                LocationId = s.LocationId,
                Start = s.StartDate,
                End = s.EndDate,
                ShiftId = s.Id,
                Timezone = s.Timezone,
                OvertimeHours = s.OvertimeHours,
                Comment = s.Comment
            });

            //We've already included this information in the conflicts.
            courtAdmins.ForEach(s => s.AwayLocation = null);
            courtAdmins.ForEach(s => s.Leave = null);
            courtAdmins.ForEach(s => s.Training = null);

            var allShiftConflicts = courtAdminEventConflicts.Concat(existingShiftConflicts).ToList();

            var lookupCode = await Db.LookupCode.AsNoTracking()
                .Where(lc => lc.Type == LookupTypes.CourtAdminRank)
                .Include(s => s.SortOrder)
                .ToListAsync();

            return courtAdmins.SelectToList(s => new ShiftAvailability
            {
                Start = start,
                End = end,
                CourtAdmin = s,
                CourtAdminId = s.Id,
                Conflicts = allShiftConflicts.WhereToList(asc => asc.CourtAdminId == s.Id)
            })
                .OrderBy(s => lookupCode.FirstOrDefault(so => so.Code == s.CourtAdmin.Rank)
                    ?.SortOrder.FirstOrDefault()
                    ?.SortOrder)
                .ThenBy(s => s.CourtAdmin.LastName)
                .ThenBy(s => s.CourtAdmin.FirstName)
                .ToList();
        }

        #region Helpers

        public async Task CalculateOvertimeHoursForShifts(List<Shift> shifts)
        {
            var courtAdminsAndDates = shifts.SelectDistinctToList(s => new { s.CourtAdminId, s.StartDate, s.Timezone });
            foreach (var courtAdminAndDate in courtAdminsAndDates)
            {
                await CalculateOvertimeHoursForCourtAdminOnDay(courtAdminAndDate.CourtAdminId, courtAdminAndDate.StartDate,
                    courtAdminAndDate.Timezone);
            }
            await Db.SaveChangesAsync();
        }

        public async Task<double> CalculateOvertimeHoursForCourtAdminOnDay(Guid? courtAdminId, DateTimeOffset startDate, string timezone)
        {
            var startOfDayInTimezone = startDate.ConvertToTimezone(timezone).DateOnly();
            var endOfDayInTimezone = startOfDayInTimezone.TranslateDateForDaylightSavings(timezone, 1);

            var shiftsForCourtAdminOnDay = await Db.Shift.Where(s =>
                s.ExpiryDate == null && s.CourtAdminId == courtAdminId &&
                startOfDayInTimezone <= s.StartDate && endOfDayInTimezone >= s.EndDate)
                .OrderBy(s => s.StartDate)
                .ToListAsync();

            if (!shiftsForCourtAdminOnDay.Any())
                return 0.0;

            foreach (var shift in shiftsForCourtAdminOnDay)
                shift.OvertimeHours = 0;

            var hoursForCourtAdminOnDay = shiftsForCourtAdminOnDay.Sum(s => s.StartDate.HourDifference(s.EndDate, s.Timezone));
            var overtimeHoursForDay = Math.Max(hoursForCourtAdminOnDay - OvertimeHoursPerDay, 0);

            //See if we have multiple shifts && a shift that is equal our OT hours
            if (shiftsForCourtAdminOnDay.Count > 1 && shiftsForCourtAdminOnDay.Any(s => s.StartDate.HourDifference(s.EndDate, s.Timezone).Equals(OvertimeHoursPerDay)))
            {
                //Place the overtime on the other shifts. This is the scenario where an outside shift(s) are created, and the OT needs to be placed on the outer shifts.
                //For example 8-9am, 9am-5pm, 5pm-6pm
                var earliestBeforeOvertimeThresholdShift = shiftsForCourtAdminOnDay.First(s =>
                    s.StartDate.HourDifference(s.EndDate, s.Timezone).Equals(OvertimeHoursPerDay));

                var outsideShifts = shiftsForCourtAdminOnDay.Where(s => s.Id != earliestBeforeOvertimeThresholdShift.Id);

                foreach (var shift in outsideShifts)
                    shift.OvertimeHours = shift.StartDate.HourDifference(shift.EndDate, shift.Timezone);
            }
            else
            {
                var overtimeHourTally = overtimeHoursForDay;
                foreach (var shift in shiftsForCourtAdminOnDay.OrderByDescending(s => s.EndDate))
                {
                    var shiftHours = shift.StartDate.HourDifference(shift.EndDate, shift.Timezone);
                    shift.OvertimeHours = Math.Min(shiftHours, overtimeHourTally);
                    overtimeHourTally -= shift.OvertimeHours;
                }
            }
            return overtimeHoursForDay;
        }

        private List<Shift> SplitLongShifts(List<Shift> shifts)
        {
            var shiftsToSplit = shifts.Where(s => s.StartDate.HourDifference(s.EndDate, s.Timezone) > OvertimeHoursPerDay).ToList();
            foreach (var shift in shiftsToSplit)
            {
                var hourDifference = shift.StartDate.HourDifference(shift.EndDate, shift.Timezone);
                shift.EndDate = shift.StartDate.TranslateDateForDaylightSavings(shift.Timezone, hoursToShift: OvertimeHoursPerDay);
                hourDifference -= (shift.EndDate - shift.StartDate).TotalHours;
                var lastEndDate = shift.EndDate;
                while (hourDifference > 0)
                {
                    var newShiftHours = Math.Min(hourDifference, OvertimeHoursPerDay);
                    var newShift = shift.Adapt<Shift>();
                    newShift.Id = 0;
                    newShift.StartDate = lastEndDate;
                    newShift.EndDate = lastEndDate.TranslateDateForDaylightSavings(shift.Timezone, hoursToShift: newShiftHours);
                    if (newShift.EndDate.Subtract(newShift.StartDate).TotalSeconds < 60)
                        break;
                    shifts.Add(newShift);
                    lastEndDate = newShift.EndDate;
                    hourDifference -= newShiftHours;
                }
            }
            return shifts;
        }

        #region Add Shift

        private async Task AddShift(Shift shift)
        {
            shift.ExpiryDate = null;
            shift.CourtAdmin = await Db.CourtAdmin.FindAsync(shift.CourtAdminId);
            shift.AnticipatedAssignment = await Db.Assignment.FindAsync(shift.AnticipatedAssignmentId);
            shift.Location = await Db.Location.FindAsync(shift.LocationId);
            await Db.Shift.AddAsync(shift);
        }

        #endregion Add Shift

        #region Override

        public async Task HandleShiftOverrides<T>(T data, DutyRosterService dutyRosterService, List<Shift> shiftConflicts) where T : CourtAdminEvent
        {
            var adjustShifts = new List<Shift>();
            var expireShiftIds = new List<int>();

            foreach (var shift in shiftConflicts)
            {
                if (data.StartDate <= shift.StartDate && data.EndDate >= shift.EndDate)
                    expireShiftIds.Add(shift.Id);
                else
                {
                    if (shift.StartDate < data.StartDate && shift.EndDate > data.EndDate)
                    {
                        var newShift = shift.Adapt<Shift>();
                        newShift.Id = 0;
                        newShift.StartDate = data.EndDate.ConvertToTimezone(data.Timezone);
                        shift.EndDate = data.StartDate.ConvertToTimezone(data.Timezone);
                        adjustShifts.Add(newShift);
                        adjustShifts.Add(shift);
                    }
                    else if (shift.EndDate > data.EndDate)
                    {
                        shift.StartDate = data.EndDate.ConvertToTimezone(data.Timezone);
                        adjustShifts.Add(shift);
                    }
                    else if (shift.StartDate < data.StartDate)
                    {
                        shift.EndDate = data.StartDate.ConvertToTimezone(data.Timezone);
                        adjustShifts.Add(shift);
                    }
                }
            }

            if (expireShiftIds.Count > 0)
                await ExpireShiftsAndDutySlots(expireShiftIds);
            if (adjustShifts.Count > 0)
                await UpdateShifts(dutyRosterService, adjustShifts);

            await Db.SaveChangesAsync();
        }

        #endregion Override

        #region Availability

        public async Task<List<ShiftConflict>> GetShiftConflicts(List<Shift> shifts)
        {
            var overlappingShifts = await CheckForShiftOverlap(shifts);
            var courtAdminEventOverlaps = await CheckCourtAdminEventsOverlap(shifts);
            return overlappingShifts.Concat(courtAdminEventOverlaps).OrderBy(o => o.Shift.StartDate).ToList();
        }

        private async Task<List<ShiftConflict>> CheckForShiftOverlap(List<Shift> shifts)
        {
            var overlappingShifts = await OverlappingShifts(shifts);
            return overlappingShifts.SelectToList(ol => new ShiftConflict
            {
                ConflictMessages = new List<string>
                {
                    ConflictingCourtAdminAndSchedule(ol.CourtAdmin, ol)
                },
                Shift = ol
            });
        }

        private async Task<List<Shift>> OverlappingShifts(List<Shift> targetShifts)
        {
            if (!targetShifts.Any()) throw new BusinessLayerException("No shifts were provided.");
            if (targetShifts.Any(a =>
                targetShifts.Any(b => a != b && b.StartDate < a.EndDate && a.StartDate < b.EndDate && a.CourtAdminId == b.CourtAdminId)))
                throw new BusinessLayerException("Shifts provided overlap with themselves.");

            var courtAdminIds = targetShifts.Select(ts => ts.CourtAdminId).Distinct().ToList();

            var conflictingShifts = new List<Shift>();
            foreach (var ts in targetShifts)
            {
                conflictingShifts.AddRange(await Db.Shift.AsNoTracking()
                    .Include(s => s.CourtAdmin)
                    .In(courtAdminIds, s => s.CourtAdminId)
                    .Where(s =>
                        s.ExpiryDate == null &&
                        s.StartDate < ts.EndDate && ts.StartDate < s.EndDate
                    ).ToListAsync());
            }

            conflictingShifts = conflictingShifts.Distinct().WhereToList(s =>
                targetShifts.Any(ts =>
                    ts.ExpiryDate == null && s.Id != ts.Id && ts.StartDate < s.EndDate && s.StartDate < ts.EndDate &&
                    ts.CourtAdminId == s.CourtAdminId) &&
                targetShifts.All(ts => ts.Id != s.Id)
            );

            return conflictingShifts;
        }

        private async Task<List<ShiftConflict>> CheckCourtAdminEventsOverlap(List<Shift> shifts)
        {
            var courtAdminEventConflicts = new List<ShiftConflict>();
            foreach (var shift in shifts)
            {
                var locationId = shift.LocationId;
                var courtAdmins = await CourtAdminService.GetCourtAdminsForShiftAvailabilityForLocation(locationId, shift.StartDate, shift.EndDate, shift.CourtAdminId);
                var courtAdmin = courtAdmins.FirstOrDefault();
                var validationErrors = new List<string>();
                if (courtAdmin == null)
                {
                    var unavailableCourtAdmin =
                        await Db.CourtAdmin.AsNoTracking().FirstOrDefaultAsync(s => s.Id == shift.CourtAdminId);
                    validationErrors.Add($"{unavailableCourtAdmin?.LastName}, {unavailableCourtAdmin?.FirstName} is not active in this location for {shift.StartDate.ConvertToTimezone(shift.Timezone).PrintFormatDate()} {shift.StartDate.ConvertToTimezone(shift.Timezone).PrintFormatTime(shift.Timezone)} to {shift.EndDate.ConvertToTimezone(shift.Timezone).PrintFormatTime(shift.Timezone)}");
                }
                else
                {
                    validationErrors.AddRange(courtAdmin!.AwayLocation.Where(aw => aw.LocationId != shift.LocationId)
                        .Select(aw => PrintCourtAdminEventConflict<CourtAdminAwayLocation>(aw.CourtAdmin,
                            aw.StartDate,
                            aw.EndDate,
                            aw.Timezone)));
                    validationErrors.AddRange(courtAdmin.Leave.Select(aw => PrintCourtAdminEventConflict<CourtAdminLeave>(
                        aw.CourtAdmin,
                        aw.StartDate,
                        aw.EndDate,
                        aw.Timezone)));
                    validationErrors.AddRange(courtAdmin.Training.Select(aw => PrintCourtAdminEventConflict<CourtAdminTraining>(
                        aw.CourtAdmin,
                        aw.StartDate,
                        aw.EndDate,
                        aw.Timezone)));
                }

                if (validationErrors.Any())
                    courtAdminEventConflicts.Add(new ShiftConflict
                    {
                        Shift = shift,
                        ConflictMessages = validationErrors
                    });
            }
            return courtAdminEventConflicts;
        }

        private async Task<List<Shift>> GetShiftsForCourtAdmins(IEnumerable<Guid> courtAdminIds, DateTimeOffset startDate, DateTimeOffset endDate) =>
            await Db.Shift.AsSingleQuery().AsNoTracking()
                    .Include(s => s.Location)
                    .In(courtAdminIds, s => s.CourtAdminId)
                    .Where(s =>
                        s.StartDate < endDate && startDate < s.EndDate &&
                        s.ExpiryDate == null)
                    .ToListAsync();

        #endregion Availability

        #region String Helpers

        private static string ConflictingCourtAdminAndSchedule(CourtAdmin courtAdmin, Shift shift)
        {
            shift.Timezone.GetTimezone().ThrowBusinessExceptionIfNull("Shift - Timezone was invalid.");
            return $"{courtAdmin.LastName}, {courtAdmin.FirstName} has a shift {shift.StartDate.ConvertToTimezone(shift.Timezone).PrintFormatDate()} {shift.StartDate.ConvertToTimezone(shift.Timezone).PrintFormatTime(shift.Timezone)} to {shift.EndDate.ConvertToTimezone(shift.Timezone).PrintFormatTime(shift.Timezone)}";
        }

        private static string PrintCourtAdminEventConflict<T>(CourtAdmin courtAdmin, DateTimeOffset start, DateTimeOffset end,
            string timezone)
        {
            timezone.GetTimezone().ThrowBusinessExceptionIfNull("CourtAdminEvent - Timezone was invalid.");
            return $"{courtAdmin.LastName}, {courtAdmin.FirstName} has {typeof(T).Name.Replace("CourtAdmin", "").ConvertCamelCaseToMultiWord()} {start.ConvertToTimezone(timezone).PrintFormatDateTime(timezone)} to {end.ConvertToTimezone(timezone).PrintFormatDateTime(timezone)}";
        }

        #endregion String Helpers

        #endregion Helpers
    }
}