using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CAS.API.helpers;
using CAS.API.helpers.extensions;
using CAS.API.infrastructure.authorization;
using CAS.API.infrastructure.exceptions;
using CAS.API.Models.DB;
using CAS.API.services.scheduling;
using CAS.COMMON.helpers.extensions;
using CAS.DB.models;
using CAS.DB.models.scheduling;
using CAS.DB.models.courtAdmin;

namespace CAS.API.services.usermanagement
{
    /// <summary>
    /// Since the Sheriff is a derived class of User, this should handle the Sheriff side of things.
    /// </summary>
    public class CourtAdminService
    {
        private ClaimsPrincipal User { get; }
        private CourtAdminDbContext Db { get; }
        private IConfiguration Configuration { get; }

        public CourtAdminService(CourtAdminDbContext db, IConfiguration configuration, IHttpContextAccessor httpContextAccessor = null)
        {
            Db = db;
            Configuration = configuration;
            User = httpContextAccessor?.HttpContext.User;
        }

        #region CourtAdmin

        public async Task<CourtAdmin> AddCourtAdmin(CourtAdmin courtAdmin)
        {
            await CheckForBlankOrDuplicateIdirName(courtAdmin.IdirName);
            await CheckForBlankOrDuplicateBadgeNumber(courtAdmin.BadgeNumber);

            courtAdmin.IdirName = courtAdmin.IdirName.ToLower();
            courtAdmin.AwayLocation = null;
            courtAdmin.ActingRank = null;
            courtAdmin.Training = null;
            courtAdmin.Leave = null;
            courtAdmin.HomeLocation = await Db.Location.FindAsync(courtAdmin.HomeLocationId);
            courtAdmin.IsEnabled = true;
            await Db.CourtAdmin.AddAsync(courtAdmin);
            await Db.SaveChangesAsync();
            return courtAdmin;
        }

        public async Task<CourtAdmin> GetCourtAdmin(Guid? id,
            string badgeNumber) =>
            await Db.CourtAdmin.AsNoTracking()
                .FirstOrDefaultAsync(s =>
                    (badgeNumber == null && id.HasValue && s.Id == id) ||
                    (!id.HasValue && s.BadgeNumber == badgeNumber));

        //Used for the Shift scheduling screen, while considering a location.
        public async Task<List<CourtAdmin>> GetCourtAdminsForShiftAvailabilityForLocation(int locationId, DateTimeOffset start, DateTimeOffset end, Guid? courtAdminId = null)
        {
            var courtAdminQuery = Db.CourtAdmin.AsNoTracking()
                .AsSplitQuery()
                .Where(s =>
                    (courtAdminId == null || courtAdminId != null && s.Id == courtAdminId) &&
                    s.IsEnabled &&
                    s.HomeLocationId == locationId ||
                    s.AwayLocation.Any(al =>
                        al.LocationId == locationId && !(al.StartDate > end || start > al.EndDate)
                                                    && al.ExpiryDate == null))
                .IncludeCourtAdminEventsBetweenDates(start, end)
                .IncludeCourtAdminActingRank();

            return await courtAdminQuery.ToListAsync();
        }

        public async Task<List<CourtAdmin>> GetFilteredCourtAdminsForTeams()
        {
            var now = DateTimeOffset.UtcNow;
            var sevenDaysFromNow = DateTimeOffset.UtcNow.AddDays(7);

            var courtAdminQuery = Db.CourtAdmin.AsNoTracking()
                .AsSplitQuery()
                .ApplyPermissionFilters(User, now, sevenDaysFromNow, Db)
                .IncludeCourtAdminEventsBetweenDates(now, sevenDaysFromNow)
                .IncludeCourtAdminActingRank();

            return await courtAdminQuery.ToListAsync();
        }

        public async Task<CourtAdmin> GetFilteredCourtAdminForTeams(Guid id)
        {
            var daysPrevious = int.Parse(Configuration.GetNonEmptyValue("DaysInPastToIncludeAwayLocationAndTraining"));
            var minDateForAwayAndTraining = DateTimeOffset.UtcNow.AddDays(-daysPrevious);
            var sevenDaysFromNow = DateTimeOffset.UtcNow.AddDays(7);

            return await Db.CourtAdmin.AsNoTracking().AsSingleQuery()
                .ApplyPermissionFilters(User, minDateForAwayAndTraining, sevenDaysFromNow, Db)
                .Include(s => s.HomeLocation)
                .Include(s => s.AwayLocation.Where(al => al.EndDate >= minDateForAwayAndTraining && al.ExpiryDate == null))
                .ThenInclude(al => al.Location)
                .Include(s => s.Leave.Where(l => l.EndDate >= minDateForAwayAndTraining && l.ExpiryDate == null))
                .ThenInclude(l => l.LeaveType)
                .Include(s => s.Training.Where(t => t.ExpiryDate == null))
                .ThenInclude(t => t.TrainingType)
                .Include(s => s.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(s => s.ActingRank.Where(ar => ar.ExpiryDate == null))
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<CourtAdmin> UpdateCourtAdmin(CourtAdmin courtAdmin, bool canEditIdir)
        {
            var savedCourtAdmin = await Db.CourtAdmin.FindAsync(courtAdmin.Id);
            savedCourtAdmin.ThrowBusinessExceptionIfNull($"{nameof(CourtAdmin)} with the id: {courtAdmin.Id} could not be found. ");

            if (courtAdmin.BadgeNumber != savedCourtAdmin.BadgeNumber)
                await CheckForBlankOrDuplicateBadgeNumber(courtAdmin.BadgeNumber);

            if (canEditIdir && savedCourtAdmin.IdirName != courtAdmin.IdirName)
            {
                await CheckForBlankOrDuplicateIdirName(courtAdmin.IdirName, courtAdmin.Id);
                courtAdmin.IdirName = courtAdmin.IdirName.ToLower();
                courtAdmin.IdirId = null;
            }
            else
            {
                courtAdmin.IdirName = savedCourtAdmin.IdirName;
                courtAdmin.IdirId = savedCourtAdmin.IdirId;
            }

            Db.Entry(savedCourtAdmin).CurrentValues.SetValues(courtAdmin);

            Db.Entry(savedCourtAdmin).Property(x => x.HomeLocationId).IsModified = false;
            Db.Entry(savedCourtAdmin).Property(x => x.IsEnabled).IsModified = false;
            Db.Entry(savedCourtAdmin).Property(x => x.Photo).IsModified = false;
            Db.Entry(savedCourtAdmin).Property(x => x.LastPhotoUpdate).IsModified = false;
            Db.Entry(savedCourtAdmin).Property(x => x.KeyCloakId).IsModified = false;
            Db.Entry(savedCourtAdmin).Property(x => x.LastLogin).IsModified = false;

            await Db.SaveChangesAsync();
            return courtAdmin;
        }

        public async Task<byte[]> GetPhoto(Guid id)
        {
            var savedCourtAdmin = await Db.CourtAdmin.FindAsync(id);
            savedCourtAdmin.ThrowBusinessExceptionIfNull($"No {nameof(CourtAdmin)} with Id: {id}");
            return savedCourtAdmin.Photo;
        }

        public async Task<CourtAdmin> UpdateCourtAdminPhoto(Guid? id, string badgeNumber, byte[] photoData)
        {
            var savedCourtAdmin = await Db.CourtAdmin.FirstOrDefaultAsync(s => (id.HasValue && s.Id == id) || (!id.HasValue && s.BadgeNumber == badgeNumber));
            savedCourtAdmin.ThrowBusinessExceptionIfNull($"No {nameof(CourtAdmin)} with Badge: {badgeNumber} or Id: {id}");
            savedCourtAdmin.Photo = photoData;
            savedCourtAdmin.LastPhotoUpdate = DateTime.UtcNow;
            await Db.SaveChangesAsync();
            return savedCourtAdmin;
        }

        public async Task UpdateSheriffHomeLocation(Guid id, int locationId)
        {
            var savedSheriff = await Db.CourtAdmin.FindAsync(id);
            savedSheriff.ThrowBusinessExceptionIfNull($"{nameof(CourtAdmin)} with the id: {id} could not be found. ");
            savedSheriff.HomeLocation = await Db.Location.FindAsync(locationId);
            savedSheriff.HomeLocation.ThrowBusinessExceptionIfNull($"{nameof(Location)} with the id: {locationId} could not be found. ");
            await Db.SaveChangesAsync();
        }

        #endregion CourtAdmin

        #region CourtAdmin Event

        public async Task<T> GetSheriffEvent<T>(int id) where T : CourtAdminEvent =>
            await Db.Set<T>().AsNoTracking().FirstOrDefaultAsync(sal => sal.Id == id);

        #endregion CourtAdmin Event

        #region CourtAdmin Location

        public async Task<CourtAdminAwayLocation> AddSheriffAwayLocation(DutyRosterService dutyRosterService, ShiftService shiftService, CourtAdminAwayLocation awayLocation, bool overrideConflicts)
        {
            ValidateStartAndEndDates(awayLocation.StartDate, awayLocation.EndDate);
            await ValidateSheriffExists(awayLocation.CourtAdminId);
            await ValidateNoOverlapAsync(dutyRosterService, shiftService, awayLocation, overrideConflicts);

            awayLocation.Location = await Db.Location.FindAsync(awayLocation.LocationId);
            awayLocation.CourtAdmin = await Db.CourtAdmin.FindAsync(awayLocation.CourtAdminId);
            await Db.SheriffAwayLocation.AddAsync(awayLocation);
            await Db.SaveChangesAsync();
            return awayLocation;
        }

        public async Task<CourtAdminAwayLocation> UpdateSheriffAwayLocation(DutyRosterService dutyRosterService, ShiftService shiftService, CourtAdminAwayLocation awayLocation, bool overrideConflicts)
        {
            ValidateStartAndEndDates(awayLocation.StartDate, awayLocation.EndDate);
            await ValidateSheriffExists(awayLocation.CourtAdminId);

            var savedAwayLocation = await Db.SheriffAwayLocation.FindAsync(awayLocation.Id);
            savedAwayLocation.ThrowBusinessExceptionIfNull($"{nameof(CourtAdminAwayLocation)} with the id: {awayLocation.Id} could not be found. ");

            if (savedAwayLocation.ExpiryDate.HasValue)
                throw new BusinessLayerException($"{nameof(CourtAdminAwayLocation)} with the id: {awayLocation.Id} has been expired");

            await ValidateNoOverlapAsync(dutyRosterService, shiftService, awayLocation, overrideConflicts, awayLocation.Id);

            Db.Entry(savedAwayLocation).CurrentValues.SetValues(awayLocation);
            Db.Entry(savedAwayLocation).Property(x => x.CourtAdminId).IsModified = false;
            Db.Entry(savedAwayLocation).Property(x => x.ExpiryDate).IsModified = false;
            Db.Entry(savedAwayLocation).Property(x => x.ExpiryReason).IsModified = false;
            await Db.SaveChangesAsync();
            return savedAwayLocation;
        }

        public async Task RemoveSheriffAwayLocation(int id, string expiryReason)
        {
            var sheriffAwayLocation = await Db.SheriffAwayLocation.FindAsync(id);
            sheriffAwayLocation.ThrowBusinessExceptionIfNull(
                $"{nameof(CourtAdminAwayLocation)} with the id: {id} could not be found. ");
            sheriffAwayLocation.ExpiryDate = DateTimeOffset.UtcNow;
            sheriffAwayLocation.ExpiryReason = expiryReason;
            await Db.SaveChangesAsync();
        }

        #endregion CourtAdmin Location

        #region CourtAdmin Acting Rank

        public async Task<CourtAdminActingRank> AddSheriffActingRank(DutyRosterService dutyRosterService, ShiftService shiftService, CourtAdminActingRank actingRank, bool overrideConflicts)
        {
            ValidateStartAndEndDates(actingRank.StartDate, actingRank.EndDate);
            await ValidateSheriffExists(actingRank.CourtAdminId);

            await ValidateSheriffActingRankExists(actingRank);

            actingRank.CourtAdmin = await Db.CourtAdmin.FindAsync(actingRank.CourtAdminId);
            await Db.SheriffActingRank.AddAsync(actingRank);
            await Db.SaveChangesAsync();
            return actingRank;
        }

        public async Task<CourtAdminActingRank> UpdateSheriffActingRank(DutyRosterService dutyRosterService, ShiftService shiftService, CourtAdminActingRank actingRank, bool overrideConflicts)
        {
            ValidateStartAndEndDates(actingRank.StartDate, actingRank.EndDate);
            await ValidateSheriffExists(actingRank.CourtAdminId);

            var savedActingRank = await Db.SheriffActingRank.FindAsync(actingRank.Id);
            savedActingRank.ThrowBusinessExceptionIfNull($"{nameof(CourtAdminActingRank)} with the id: {actingRank.Id} could not be found. ");

            if (savedActingRank.ExpiryDate.HasValue)
                throw new BusinessLayerException($"{nameof(CourtAdminActingRank)} with the id: {actingRank.Id} has been expired");

            await ValidateSheriffActingRankExists(actingRank);

            Db.Entry(savedActingRank).CurrentValues.SetValues(actingRank);
            Db.Entry(savedActingRank).Property(x => x.CourtAdminId).IsModified = false;
            Db.Entry(savedActingRank).Property(x => x.ExpiryDate).IsModified = false;
            Db.Entry(savedActingRank).Property(x => x.ExpiryReason).IsModified = false;
            await Db.SaveChangesAsync();
            return savedActingRank;
        }

        public async Task RemoveSheriffActingRank(int id, string expiryReason)
        {
            var sheriffActingRank = await Db.SheriffActingRank.FindAsync(id);
            sheriffActingRank.ThrowBusinessExceptionIfNull(
                $"{nameof(CourtAdminActingRank)} with the id: {id} could not be found. ");
            sheriffActingRank.ExpiryDate = DateTimeOffset.UtcNow;
            sheriffActingRank.ExpiryReason = expiryReason;
            await Db.SaveChangesAsync();
        }

        #endregion CourtAdmin Acting Rank

        #region CourtAdmin Leave

        public async Task<CourtAdminLeave> AddSheriffLeave(DutyRosterService dutyRosterService, ShiftService shiftService, CourtAdminLeave sheriffLeave, bool overrideConflicts)
        {
            ValidateStartAndEndDates(sheriffLeave.StartDate, sheriffLeave.EndDate);
            await ValidateSheriffExists(sheriffLeave.CourtAdminId);
            await ValidateNoOverlapAsync(dutyRosterService, shiftService, sheriffLeave, overrideConflicts);

            sheriffLeave.LeaveType = await Db.LookupCode.FindAsync(sheriffLeave.LeaveTypeId);
            sheriffLeave.CourtAdmin = await Db.CourtAdmin.FindAsync(sheriffLeave.CourtAdminId);
            await Db.SheriffLeave.AddAsync(sheriffLeave);
            await Db.SaveChangesAsync();
            return sheriffLeave;
        }

        public async Task<CourtAdminLeave> UpdateSheriffLeave(DutyRosterService dutyRosterService, ShiftService shiftService, CourtAdminLeave sheriffLeave, bool overrideConflicts)
        {
            ValidateStartAndEndDates(sheriffLeave.StartDate, sheriffLeave.EndDate);
            await ValidateSheriffExists(sheriffLeave.CourtAdminId);

            var savedLeave = await Db.SheriffLeave.FindAsync(sheriffLeave.Id);
            savedLeave.ThrowBusinessExceptionIfNull(
                $"{nameof(CourtAdminLeave)} with the id: {sheriffLeave.Id} could not be found. ");

            if (savedLeave.ExpiryDate.HasValue)
                throw new BusinessLayerException($"{nameof(CourtAdminLeave)} with the id: {sheriffLeave.Id} has been expired");

            await ValidateNoOverlapAsync(dutyRosterService, shiftService, sheriffLeave, overrideConflicts, sheriffLeave.Id);

            Db.Entry(savedLeave).CurrentValues.SetValues(sheriffLeave);
            Db.Entry(savedLeave).Property(x => x.CourtAdminId).IsModified = false;
            Db.Entry(savedLeave).Property(x => x.ExpiryDate).IsModified = false;
            Db.Entry(savedLeave).Property(x => x.ExpiryReason).IsModified = false;

            await Db.SaveChangesAsync();
            return savedLeave;
        }

        public async Task RemoveSheriffLeave(int id, string expiryReason)
        {
            var sheriffLeave = await Db.SheriffLeave.FindAsync(id);
            sheriffLeave.ThrowBusinessExceptionIfNull(
                $"{nameof(CourtAdminLeave)} with the id: {sheriffLeave.Id} could not be found. ");
            sheriffLeave.ExpiryDate = DateTimeOffset.UtcNow;
            sheriffLeave.ExpiryReason = expiryReason;
            await Db.SaveChangesAsync();
        }

        #endregion CourtAdmin Leave

        #region CourtAdmin Training

        public async Task<List<CourtAdmin>> GetSheriffsTraining()
        {
            var daysPrevious = int.Parse(Configuration.GetNonEmptyValue("DaysInPastToIncludeAwayLocationAndTraining"));
            var minDateForAwayAndTraining = DateTimeOffset.UtcNow.AddDays(-daysPrevious);
            var sevenDaysFromNow = DateTimeOffset.UtcNow.AddDays(7);
                        
            var sheriffQuery = Db.CourtAdmin.AsNoTracking()
                .AsSplitQuery()
                .ApplyPermissionFilters(User, minDateForAwayAndTraining, sevenDaysFromNow, Db)
                .Include(s => s.Training.Where(t => t.ExpiryDate == null))
                .ThenInclude(t => t.TrainingType)
                .Where(t => t.Training.Count > 0);

            return await sheriffQuery.ToListAsync();
        }

        public async Task<CourtAdminTraining> AddSheriffTraining(DutyRosterService dutyRosterService, ShiftService shiftService, CourtAdminTraining sheriffTraining, bool overrideConflicts)
        {
            ValidateStartAndEndDates(sheriffTraining.StartDate, sheriffTraining.EndDate);
            await ValidateSheriffExists(sheriffTraining.CourtAdminId);
            await ValidateNoOverlapAsync(dutyRosterService, shiftService, sheriffTraining, overrideConflicts);

            sheriffTraining.CourtAdmin = await Db.CourtAdmin.FindAsync(sheriffTraining.CourtAdminId);
            sheriffTraining.TrainingType = await Db.LookupCode.FindAsync(sheriffTraining.TrainingTypeId);
            await Db.SheriffTraining.AddAsync(sheriffTraining);
            await Db.SaveChangesAsync();
            return sheriffTraining;
        }

        public async Task<CourtAdminTraining> UpdateSheriffTraining(DutyRosterService dutyRosterService, ShiftService shiftService, CourtAdminTraining sheriffTraining, bool overrideConflicts)
        {
            ValidateStartAndEndDates(sheriffTraining.StartDate, sheriffTraining.EndDate);
            await ValidateSheriffExists(sheriffTraining.CourtAdminId);

            var savedTraining = await Db.SheriffTraining.FindAsync(sheriffTraining.Id);
            savedTraining.ThrowBusinessExceptionIfNull(
                $"{nameof(CourtAdminTraining)} with the id: {sheriffTraining.Id} could not be found. ");

            if (savedTraining.ExpiryDate.HasValue)
                throw new BusinessLayerException($"{nameof(CourtAdminTraining)} with the id: {sheriffTraining.Id} has been expired");

            await ValidateNoOverlapAsync(dutyRosterService, shiftService, sheriffTraining, overrideConflicts, sheriffTraining.Id);

            Db.Entry(savedTraining).CurrentValues.SetValues(sheriffTraining);
            Db.Entry(savedTraining).Property(x => x.CourtAdminId).IsModified = false;
            Db.Entry(savedTraining).Property(x => x.ExpiryDate).IsModified = false;
            Db.Entry(savedTraining).Property(x => x.ExpiryReason).IsModified = false;

            await Db.SaveChangesAsync();
            return savedTraining;
        }

        public async Task RemoveSheriffTraining(int id, string expiryReason)
        {
            var sheriffTraining = await Db.SheriffTraining.FindAsync(id);
            sheriffTraining.ThrowBusinessExceptionIfNull(
                $"{nameof(CourtAdminTraining)} with the id: {id} could not be found. ");
            sheriffTraining.ExpiryDate = DateTimeOffset.UtcNow;
            sheriffTraining.ExpiryReason = expiryReason;
            await Db.SaveChangesAsync();
        }

        #endregion CourtAdmin Training

        #region Helpers

        #region Validation

        private async Task CheckForBlankOrDuplicateIdirName(string idirName, Guid? excludeSheriffId = null)
        {
            if (string.IsNullOrEmpty(idirName))
                throw new BusinessLayerException($"Missing {nameof(idirName)}.");

            var existingSheriffWithIdir = await Db.CourtAdmin.FirstOrDefaultAsync(s => s.IdirName.ToLower() == idirName.ToLower() && s.Id != excludeSheriffId);
            if (existingSheriffWithIdir != null)
                throw new BusinessLayerException(
                    $"Sheriff {existingSheriffWithIdir.LastName}, {existingSheriffWithIdir.FirstName} has IDIR name: {existingSheriffWithIdir.IdirName}");
        }

        private async Task CheckForBlankOrDuplicateBadgeNumber(string badgeNumber)
        {
            if (string.IsNullOrEmpty(badgeNumber))
                throw new BusinessLayerException($"Missing {nameof(badgeNumber)}.");

            var existingSheriffWithBadge = await Db.CourtAdmin.FirstOrDefaultAsync(s => s.BadgeNumber == badgeNumber);
            if (existingSheriffWithBadge != null)
                throw new BusinessLayerException(
                    $"Sheriff {existingSheriffWithBadge.LastName}, {existingSheriffWithBadge.FirstName} already has badge number: {badgeNumber}");
        }

        private void ValidateStartAndEndDates(DateTimeOffset startDate, DateTimeOffset endDate)
        {
            if (startDate >= endDate) throw new BusinessLayerException("The from cannot be on or after the to. ");
        }

        private async Task ValidateSheriffExists(Guid sheriffId)
        {
            if (!await Db.CourtAdmin.AsNoTracking().AnyAsync(s => s.Id == sheriffId))
                throw new BusinessLayerException($"Sheriff with id: {sheriffId} does not exist.");
        }

        private async Task ValidateSheriffActingRankExists(CourtAdminActingRank actingRank)
        {
            var conflictingRanks = await Db.SheriffActingRank.AsNoTracking().Where(sal =>
                     sal.Id != actingRank.Id &&
                     sal.CourtAdminId == actingRank.CourtAdminId &&
                    (sal.StartDate < actingRank.EndDate && actingRank.StartDate < sal.EndDate) &&
                    sal.ExpiryDate == null).ToListAsync();
            if (conflictingRanks.Count > 0)
            {
                var start = actingRank.StartDate.ConvertToTimezone(actingRank.Timezone);
                var end = actingRank.EndDate.ConvertToTimezone(actingRank.Timezone);
                throw new BusinessLayerException($"Acting Rank date-range({start.Year}-{start.Month}-{start.Day}" +
                    $" to {end.Year}-{end.Month}-{end.Day}) has coflict with current acting ranks.");
            }
        }

        #region Overlap Detection && Override

        private async Task ValidateNoOverlapAsync<T>(DutyRosterService dutyRosterService, ShiftService shiftService, T data, bool overrideConflicts, int? updateOnlyId = null) where T : CourtAdminEvent
        {
            var sheriffEventConflicts = new List<CourtAdminEvent>
            {
                await LookForSheriffEventConflictAsync(Db.SheriffLeave, data, updateOnlyId),
                await LookForSheriffEventConflictAsync(Db.SheriffTraining, data, updateOnlyId),
                await LookForSheriffEventConflictAsync(Db.SheriffAwayLocation, data, updateOnlyId)
            }.WhereToList(se => se != null);

            var shiftConflicts = await Db.Shift.AsNoTracking().Include(d => d.Location)
                .Where(sal =>
                sal.SheriffId == data.CourtAdminId && (sal.StartDate < data.EndDate && data.StartDate < sal.EndDate) &&
                sal.ExpiryDate == null).ToListAsync();

            sheriffEventConflicts = AllowDifferentTimezonesCloseTimeGap(sheriffEventConflicts, data);

            if (!overrideConflicts)
                DisplayConflicts(data.CourtAdminId, sheriffEventConflicts, shiftConflicts);
            else
                await OverrideConflicts(data, dutyRosterService, shiftService, sheriffEventConflicts, shiftConflicts);
        }

        /// <summary>
        /// This is built for the case where we create an away location in -8 for say a day (00:00:00 to 23:59:00 PST),
        /// then immediately the day after schedule training in -7 (00:00:00 to 23:59:00 MST).
        /// The two overlap, because the EndDate for away location in PST is 00:59:00 MST.
        /// This same scenario could happen moving backwards in time. 00:00:00 in -7 is 23:00:00 in -8.
        /// </summary>
        private List<CourtAdminEvent> AllowDifferentTimezonesCloseTimeGap<T>(List<CourtAdminEvent> sheriffEventConflicts, T data) where T : CourtAdminEvent
        {
            if (sheriffEventConflicts.All(sec => sec.Timezone == data.Timezone))
                return sheriffEventConflicts;

            data.StartDate = data.StartDate.ConvertToTimezone(data.Timezone);
            data.EndDate = data.EndDate.ConvertToTimezone(data.Timezone);

            var newStartDate = data.StartDate.Hour == 0 && data.StartDate.Minute == 0
                ? data.StartDate.TranslateDateForDaylightSavings(data.Timezone, hoursToShift: 1)
                : data.StartDate;

            var newEndDate = data.EndDate.Hour == 23 && data.EndDate.Minute == 59
                ? data.EndDate.TranslateDateForDaylightSavings(data.Timezone, hoursToShift: -1)
                : data.EndDate;

            sheriffEventConflicts = sheriffEventConflicts.WhereToList(sec => sec.StartDate < newEndDate && newStartDate < sec.EndDate);

            return sheriffEventConflicts;
        }

        private void DisplayConflicts(Guid sheriffId, List<CourtAdminEvent> sheriffEventConflicts, List<Shift> shiftConflicts)
        {
            DateTimeOffset startDate;
            string startDateFormatted;
            string endDateFormatted;
            var conflictErrors = new List<(DateTimeOffset start, string message)>();

            foreach (var eventConflict in sheriffEventConflicts)
            {
                if (eventConflict is CourtAdminAwayLocation sheriffAwayLocation)
                {
                    //Calculate the start and end date for the away location.
                    var awayLocationTimezone = Db.Location.AsNoTracking()
                        .FirstOrDefault(al => al.Id == sheriffAwayLocation.LocationId)?.Timezone;
                    startDate = sheriffAwayLocation.StartDate.ConvertToTimezone(awayLocationTimezone);
                    startDateFormatted = startDate.PrintFormatDateTime(awayLocationTimezone);
                    endDateFormatted = sheriffAwayLocation.EndDate.ConvertToTimezone(awayLocationTimezone)
                        .PrintFormatDateTime(awayLocationTimezone);
                }
                else
                {
                    //Calculate the start and end date for the user's home location id.
                    var homeLocationId = Db.CourtAdmin.AsNoTracking().FirstOrDefault(s => s.Id == sheriffId)
                        ?.HomeLocationId;
                    var homeLocationTimezone = Db.Location.AsNoTracking()
                        .FirstOrDefault(al => al.Id == homeLocationId.Value)?.Timezone;
                    startDate = eventConflict.StartDate.ConvertToTimezone(homeLocationTimezone);
                    startDateFormatted = startDate.PrintFormatDateTime(homeLocationTimezone);
                    endDateFormatted = eventConflict.EndDate.ConvertToTimezone(homeLocationTimezone)
                        .PrintFormatDateTime(homeLocationTimezone);
                }
                conflictErrors.Add((startDate, $"Overlaps with existing {eventConflict.GetType().Name.ConvertCamelCaseToMultiWord()}: {startDateFormatted} to {endDateFormatted}"));
            }

            foreach (var shiftConflict in shiftConflicts)
            {
                var date = shiftConflict.StartDate.ConvertToTimezone(shiftConflict.Timezone).PrintFormatDate();
                startDate = shiftConflict.StartDate.ConvertToTimezone(shiftConflict.Timezone);
                startDateFormatted = startDate.PrintFormatTime(shiftConflict.Timezone);
                endDateFormatted = shiftConflict.EndDate.ConvertToTimezone(shiftConflict.Timezone).PrintFormatTime(shiftConflict.Timezone);
                conflictErrors.Add((startDate,
                    $"Overlaps with existing {nameof(Shift).ConvertCamelCaseToMultiWord()} @ {shiftConflict.Location.Name}: {date} {startDateFormatted} to {endDateFormatted}"));
            }

            if (conflictErrors.Any())
                throw new BusinessLayerException(conflictErrors.OrderBy(ce => ce.start)
                    .Select(s => s.message)
                    .ToStringWithPipes());
        }

        private async Task OverrideConflicts<T>(T data, DutyRosterService dutyRosterService, ShiftService shiftService, List<CourtAdminEvent> sheriffEventConflicts, List<Shift> shiftConflicts) where T : CourtAdminEvent
        {
            foreach (var eventConflict in sheriffEventConflicts)
            {
                switch (eventConflict)
                {
                    case CourtAdminAwayLocation _:
                        var sheriffAwayLocation = Db.SheriffAwayLocation.First(sl => sl.Id == eventConflict.Id);
                        sheriffAwayLocation.ExpiryDate = DateTimeOffset.UtcNow;
                        sheriffAwayLocation.ExpiryReason = "OVERRIDE";
                        break;

                    case CourtAdminTraining _:
                        var sheriffTraining = Db.SheriffTraining.First(sl => sl.Id == eventConflict.Id);
                        sheriffTraining.ExpiryDate = DateTimeOffset.UtcNow;
                        sheriffTraining.ExpiryReason = "OVERRIDE";
                        break;

                    case CourtAdminLeave _:
                        var sheriffLeave = Db.SheriffLeave.First(sl => sl.Id == eventConflict.Id);
                        sheriffLeave.ExpiryDate = DateTimeOffset.UtcNow;
                        sheriffLeave.ExpiryReason = "OVERRIDE";
                        break;
                }
            }

            await shiftService.HandleShiftOverrides(data, dutyRosterService, shiftConflicts);
        }

        #endregion Overlap Detection && Override

        private async Task<T1> LookForSheriffEventConflictAsync<T1, T2>(DbSet<T1> dbSet, T2 data, int? updateOnlyId = null) where T1 : CourtAdminEvent where T2 : CourtAdminEvent
        {
            return await dbSet.AsNoTracking().FirstOrDefaultAsync(sal =>
                sal.CourtAdminId == data.CourtAdminId && (sal.StartDate < data.EndDate && data.StartDate < sal.EndDate) &&
                sal.ExpiryDate == null &&
                (typeof(T1) != typeof(T2) ||
                (typeof(T1) == typeof(T2) && (!updateOnlyId.HasValue || updateOnlyId.HasValue && sal.Id != updateOnlyId))));
        }

        #endregion Validation

        #endregion Helpers
    }
}