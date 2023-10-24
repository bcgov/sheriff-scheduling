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
        private CourtAdminService SheriffService { get; }
        public double OvertimeHoursPerDay { get; }

        public ShiftService(CourtAdminDbContext db, CourtAdminService sheriffService, IConfiguration configuration)
        {
            Db = db;
            SheriffService = sheriffService;
            OvertimeHoursPerDay = double.Parse(configuration.GetNonEmptyValue("OvertimeHoursPerDay"));
        }

        public async Task<List<Shift>> GetShiftsForLocation(int locationId, DateTimeOffset start, DateTimeOffset end, bool includeDuties)
        {
            var lookupCode = await Db.LookupCode.AsNoTracking()
                .Where(lc => lc.Type == LookupTypes.SheriffRank)
                .Include(s => s.SortOrder)
                .ToListAsync();

            var rankStartDate = DateTimeOffset.UtcNow;

            var shifts = await Db.Shift.AsSingleQuery().AsNoTracking()
                .Include(s => s.Location)
                .Include(s => s.Sheriff)
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
                        ds.SheriffId == shift.SheriffId && ds.StartDate < shift.EndDate &&
                        shift.StartDate < ds.EndDate)
                    .ToList();

            return shifts.OrderBy(s => lookupCode.FirstOrDefault(so => so.Code == s.Sheriff?.Rank)
                ?.SortOrder.FirstOrDefault()
                ?.SortOrder)
            .ThenBy(s => s.Sheriff.LastName)
            .ThenBy(s => s.Sheriff.FirstName)
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

                savedShift.Sheriff = await Db.CourtAdmin.FindAsync(shift.SheriffId);
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
                    d.SheriffId == shift.SheriffId &&
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

            var sheriffsAvailableAtLocation = await SheriffService.GetCourtAdminsForShiftAvailabilityForLocation(locationId, targetStartDate, targetEndDate);
            var sheriffIds = sheriffsAvailableAtLocation.SelectDistinctToList(s => s.Id);

            var shiftsToImport = Db.Shift
                .Include(s => s.Location)
                .Include(s => s.Sheriff)
                .AsNoTracking()
                .In(sheriffIds, s => s.SheriffId)
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
                                                                             ts.Shift.SheriffId == s.SheriffId));

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
            var sheriffs = await SheriffService.GetCourtAdminsForShiftAvailabilityForLocation(locationId, start, end);

            //Include sheriffs that have a shift, but their home location / away location doesn't match.
            //Grey out on the GUI if HomeLocationId and AwayLocation doesn't match.
            var sheriffIdsFromShifts = await Db.Shift.AsNoTracking()
                .Where(s => s.StartDate < end && start < s.EndDate && s.ExpiryDate == null &&
                            s.LocationId == locationId)
                .Select(s => s.SheriffId)
                .ToListAsync();

            var sheriffsOutOfLocationWithShiftIds = sheriffIdsFromShifts.Except(sheriffs.Select(s => s.Id));

            //Note their AwayLocation, Leave, Training should be entirely empty, this is intentional.
            var sheriffsOutOfLocationWithShift = await
               Db.CourtAdmin.AsNoTracking()
                   .Include(s => s.HomeLocation)
                   .In(sheriffsOutOfLocationWithShiftIds,
                       s => s.Id)
                   .Where(s => s.IsEnabled)
                   .ToListAsync();

            sheriffs = sheriffs.Concat(sheriffsOutOfLocationWithShift).ToList();

            var shiftsForSheriffs = await GetShiftsForSheriffs(sheriffs.Select(s => s.Id), start, end);

            var sheriffEventConflicts = new List<ShiftAvailabilityConflict>();
            sheriffs.ForEach(sheriff =>
            {
                sheriffEventConflicts.AddRange(sheriff.AwayLocation.Select(s => new ShiftAvailabilityConflict
                {
                    Conflict = ShiftConflictType.AwayLocation,
                    SheriffId = sheriff.Id,
                    Start = s.StartDate,
                    End = s.EndDate,
                    LocationId = s.LocationId,
                    Location = s.Location,
                    Timezone = s.Timezone,
                    Comment = s.Comment
                }));
                sheriffEventConflicts.AddRange(sheriff.Leave.Select(s => new ShiftAvailabilityConflict
                {
                    Conflict = ShiftConflictType.Leave,
                    SheriffId = sheriff.Id,
                    Start = s.StartDate,
                    End = s.EndDate,
                    Timezone = s.Timezone,
                    SheriffEventType = s.LeaveType?.Code,
                    Comment = s.Comment
                }));
                sheriffEventConflicts.AddRange(sheriff.Training.Select(s => new ShiftAvailabilityConflict
                {
                    Conflict = ShiftConflictType.Training,
                    SheriffId = sheriff.Id,
                    Start = s.StartDate,
                    End = s.EndDate,
                    Timezone = s.Timezone,
                    SheriffEventType = s.TrainingType?.Code,
                    Comment = s.Note
                }));
            });

            var existingShiftConflicts = shiftsForSheriffs.Select(s => new ShiftAvailabilityConflict
            {
                Conflict = ShiftConflictType.Scheduled,
                SheriffId = s.SheriffId,
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
            sheriffs.ForEach(s => s.AwayLocation = null);
            sheriffs.ForEach(s => s.Leave = null);
            sheriffs.ForEach(s => s.Training = null);

            var allShiftConflicts = sheriffEventConflicts.Concat(existingShiftConflicts).ToList();

            var lookupCode = await Db.LookupCode.AsNoTracking()
                .Where(lc => lc.Type == LookupTypes.SheriffRank)
                .Include(s => s.SortOrder)
                .ToListAsync();

            return sheriffs.SelectToList(s => new ShiftAvailability
            {
                Start = start,
                End = end,
                Sheriff = s,
                SheriffId = s.Id,
                Conflicts = allShiftConflicts.WhereToList(asc => asc.SheriffId == s.Id)
            })
                .OrderBy(s => lookupCode.FirstOrDefault(so => so.Code == s.Sheriff.Rank)
                    ?.SortOrder.FirstOrDefault()
                    ?.SortOrder)
                .ThenBy(s => s.Sheriff.LastName)
                .ThenBy(s => s.Sheriff.FirstName)
                .ToList();
        }

        #region Helpers

        public async Task CalculateOvertimeHoursForShifts(List<Shift> shifts)
        {
            var sheriffsAndDates = shifts.SelectDistinctToList(s => new { s.SheriffId, s.StartDate, s.Timezone });
            foreach (var sheriffAndDate in sheriffsAndDates)
            {
                await CalculateOvertimeHoursForSheriffOnDay(sheriffAndDate.SheriffId, sheriffAndDate.StartDate,
                    sheriffAndDate.Timezone);
            }
            await Db.SaveChangesAsync();
        }

        public async Task<double> CalculateOvertimeHoursForSheriffOnDay(Guid? sheriffId, DateTimeOffset startDate, string timezone)
        {
            var startOfDayInTimezone = startDate.ConvertToTimezone(timezone).DateOnly();
            var endOfDayInTimezone = startOfDayInTimezone.TranslateDateForDaylightSavings(timezone, 1);

            var shiftsForSheriffOnDay = await Db.Shift.Where(s =>
                s.ExpiryDate == null && s.SheriffId == sheriffId &&
                startOfDayInTimezone <= s.StartDate && endOfDayInTimezone >= s.EndDate)
                .OrderBy(s => s.StartDate)
                .ToListAsync();

            if (!shiftsForSheriffOnDay.Any())
                return 0.0;

            foreach (var shift in shiftsForSheriffOnDay)
                shift.OvertimeHours = 0;

            var hoursForSheriffOnDay = shiftsForSheriffOnDay.Sum(s => s.StartDate.HourDifference(s.EndDate, s.Timezone));
            var overtimeHoursForDay = Math.Max(hoursForSheriffOnDay - OvertimeHoursPerDay, 0);

            //See if we have multiple shifts && a shift that is equal our OT hours
            if (shiftsForSheriffOnDay.Count > 1 && shiftsForSheriffOnDay.Any(s => s.StartDate.HourDifference(s.EndDate, s.Timezone).Equals(OvertimeHoursPerDay)))
            {
                //Place the overtime on the other shifts. This is the scenario where an outside shift(s) are created, and the OT needs to be placed on the outer shifts.
                //For example 8-9am, 9am-5pm, 5pm-6pm
                var earliestBeforeOvertimeThresholdShift = shiftsForSheriffOnDay.First(s =>
                    s.StartDate.HourDifference(s.EndDate, s.Timezone).Equals(OvertimeHoursPerDay));

                var outsideShifts = shiftsForSheriffOnDay.Where(s => s.Id != earliestBeforeOvertimeThresholdShift.Id);

                foreach (var shift in outsideShifts)
                    shift.OvertimeHours = shift.StartDate.HourDifference(shift.EndDate, shift.Timezone);
            }
            else
            {
                var overtimeHourTally = overtimeHoursForDay;
                foreach (var shift in shiftsForSheriffOnDay.OrderByDescending(s => s.EndDate))
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
            shift.Sheriff = await Db.CourtAdmin.FindAsync(shift.SheriffId);
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
            var sheriffEventOverlaps = await CheckSheriffEventsOverlap(shifts);
            return overlappingShifts.Concat(sheriffEventOverlaps).OrderBy(o => o.Shift.StartDate).ToList();
        }

        private async Task<List<ShiftConflict>> CheckForShiftOverlap(List<Shift> shifts)
        {
            var overlappingShifts = await OverlappingShifts(shifts);
            return overlappingShifts.SelectToList(ol => new ShiftConflict
            {
                ConflictMessages = new List<string>
                {
                    ConflictingSheriffAndSchedule(ol.Sheriff, ol)
                },
                Shift = ol
            });
        }

        private async Task<List<Shift>> OverlappingShifts(List<Shift> targetShifts)
        {
            if (!targetShifts.Any()) throw new BusinessLayerException("No shifts were provided.");
            if (targetShifts.Any(a =>
                targetShifts.Any(b => a != b && b.StartDate < a.EndDate && a.StartDate < b.EndDate && a.SheriffId == b.SheriffId)))
                throw new BusinessLayerException("Shifts provided overlap with themselves.");

            var sheriffIds = targetShifts.Select(ts => ts.SheriffId).Distinct().ToList();

            var conflictingShifts = new List<Shift>();
            foreach (var ts in targetShifts)
            {
                conflictingShifts.AddRange(await Db.Shift.AsNoTracking()
                    .Include(s => s.Sheriff)
                    .In(sheriffIds, s => s.SheriffId)
                    .Where(s =>
                        s.ExpiryDate == null &&
                        s.StartDate < ts.EndDate && ts.StartDate < s.EndDate
                    ).ToListAsync());
            }

            conflictingShifts = conflictingShifts.Distinct().WhereToList(s =>
                targetShifts.Any(ts =>
                    ts.ExpiryDate == null && s.Id != ts.Id && ts.StartDate < s.EndDate && s.StartDate < ts.EndDate &&
                    ts.SheriffId == s.SheriffId) &&
                targetShifts.All(ts => ts.Id != s.Id)
            );

            return conflictingShifts;
        }

        private async Task<List<ShiftConflict>> CheckSheriffEventsOverlap(List<Shift> shifts)
        {
            var sheriffEventConflicts = new List<ShiftConflict>();
            foreach (var shift in shifts)
            {
                var locationId = shift.LocationId;
                var sheriffs = await SheriffService.GetCourtAdminsForShiftAvailabilityForLocation(locationId, shift.StartDate, shift.EndDate, shift.SheriffId);
                var sheriff = sheriffs.FirstOrDefault();
                var validationErrors = new List<string>();
                if (sheriff == null)
                {
                    var unavailableSheriff =
                        await Db.CourtAdmin.AsNoTracking().FirstOrDefaultAsync(s => s.Id == shift.SheriffId);
                    validationErrors.Add($"{unavailableSheriff?.LastName}, {unavailableSheriff?.FirstName} is not active in this location for {shift.StartDate.ConvertToTimezone(shift.Timezone).PrintFormatDate()} {shift.StartDate.ConvertToTimezone(shift.Timezone).PrintFormatTime(shift.Timezone)} to {shift.EndDate.ConvertToTimezone(shift.Timezone).PrintFormatTime(shift.Timezone)}");
                }
                else
                {
                    validationErrors.AddRange(sheriff!.AwayLocation.Where(aw => aw.LocationId != shift.LocationId)
                        .Select(aw => PrintSheriffEventConflict<CourtAdminAwayLocation>(aw.CourtAdmin,
                            aw.StartDate,
                            aw.EndDate,
                            aw.Timezone)));
                    validationErrors.AddRange(sheriff.Leave.Select(aw => PrintSheriffEventConflict<CourtAdminLeave>(
                        aw.CourtAdmin,
                        aw.StartDate,
                        aw.EndDate,
                        aw.Timezone)));
                    validationErrors.AddRange(sheriff.Training.Select(aw => PrintSheriffEventConflict<CourtAdminTraining>(
                        aw.CourtAdmin,
                        aw.StartDate,
                        aw.EndDate,
                        aw.Timezone)));
                }

                if (validationErrors.Any())
                    sheriffEventConflicts.Add(new ShiftConflict
                    {
                        Shift = shift,
                        ConflictMessages = validationErrors
                    });
            }
            return sheriffEventConflicts;
        }

        private async Task<List<Shift>> GetShiftsForSheriffs(IEnumerable<Guid> sheriffIds, DateTimeOffset startDate, DateTimeOffset endDate) =>
            await Db.Shift.AsSingleQuery().AsNoTracking()
                    .Include(s => s.Location)
                    .In(sheriffIds, s => s.SheriffId)
                    .Where(s =>
                        s.StartDate < endDate && startDate < s.EndDate &&
                        s.ExpiryDate == null)
                    .ToListAsync();

        #endregion Availability

        #region String Helpers

        private static string ConflictingSheriffAndSchedule(CourtAdmin sheriff, Shift shift)
        {
            shift.Timezone.GetTimezone().ThrowBusinessExceptionIfNull("Shift - Timezone was invalid.");
            return $"{sheriff.LastName}, {sheriff.FirstName} has a shift {shift.StartDate.ConvertToTimezone(shift.Timezone).PrintFormatDate()} {shift.StartDate.ConvertToTimezone(shift.Timezone).PrintFormatTime(shift.Timezone)} to {shift.EndDate.ConvertToTimezone(shift.Timezone).PrintFormatTime(shift.Timezone)}";
        }

        private static string PrintSheriffEventConflict<T>(CourtAdmin sheriff, DateTimeOffset start, DateTimeOffset end,
            string timezone)
        {
            timezone.GetTimezone().ThrowBusinessExceptionIfNull("SheriffEvent - Timezone was invalid.");
            return $"{sheriff.LastName}, {sheriff.FirstName} has {typeof(T).Name.Replace("Sheriff", "").ConvertCamelCaseToMultiWord()} {start.ConvertToTimezone(timezone).PrintFormatDateTime(timezone)} to {end.ConvertToTimezone(timezone).PrintFormatDateTime(timezone)}";
        }

        #endregion String Helpers

        #endregion Helpers
    }
}