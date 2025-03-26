using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SS.Api.helpers;
using SS.Api.helpers.extensions;
using SS.Api.infrastructure.authorization;
using SS.Api.infrastructure.exceptions;
using SS.Api.models.dto.generated;
using SS.Api.Models.DB;
using SS.Api.services.scheduling;
using SS.Common.helpers.extensions;
using SS.Db.models;
using SS.Db.models.auth;
using SS.Db.models.scheduling;
using SS.Db.models.sheriff;

namespace SS.Api.services.usermanagement
{
    /// <summary>
    /// Since the Sheriff is a derived class of User, this should handle the Sheriff side of things.
    /// </summary>
    public class SheriffService
    {
        private ClaimsPrincipal User { get; }
        private SheriffDbContext Db { get; }
        private IConfiguration Configuration { get; }

        public SheriffService(SheriffDbContext db, IConfiguration configuration, IHttpContextAccessor httpContextAccessor = null)
        {
            Db = db;
            Configuration = configuration;
            User = httpContextAccessor?.HttpContext.User;
        }

        #region Sheriff

        public async Task<Sheriff> AddSheriff(Sheriff sheriff)
        {
            await CheckForBlankOrDuplicateIdirName(sheriff.IdirName);
            await CheckForBlankOrDuplicateBadgeNumber(sheriff.BadgeNumber);

            sheriff.IdirName = sheriff.IdirName.ToLower();
            sheriff.AwayLocation = null;
            sheriff.ActingRank = null;
            sheriff.Training = null;
            sheriff.Leave = null;
            sheriff.HomeLocation = await Db.Location.FindAsync(sheriff.HomeLocationId);
            sheriff.IsEnabled = true;
            await Db.Sheriff.AddAsync(sheriff);
            await Db.SaveChangesAsync();

            await PopulateStandardTrainings(sheriff);
            
            return sheriff;
        }

        // https://jira.justice.gov.bc.ca/browse/SS-818
        private async Task PopulateStandardTrainings(Sheriff sheriff)
        {
            var standardTrainings = new List<SheriffTraining>
            {
                // Emergency Vehicle Operator Course - Motor Vehicle Act (Justice Institute of British Columbia)
                new SheriffTraining { TrainingTypeId = 1718748, ExpiryDate = DateTime.UtcNow.AddYears(100) },
                // NARCAN Training
                new SheriffTraining { TrainingTypeId = 1716961, ExpiryDate = new DateTime(DateTime.UtcNow.Year, 12, 31, 23, 59, 59) },
                // CEW, Conductive Energy Weapon (Canadian Police Knowledge Network)
                new SheriffTraining { TrainingTypeId = 1718646, ExpiryDate =  new DateTime(DateTime.UtcNow.Year, 12, 31, 23, 59, 59) },
                // Annual Review of Standards of Conduct
                new SheriffTraining { TrainingTypeId = 1716948, ExpiryDate = DateTime.UtcNow.Month > 5 || (DateTime.UtcNow.Month == 5 && DateTime.UtcNow.Day > 31) 
                    ? new DateTime(DateTime.UtcNow.Year + 1, 5, 31, 23, 59, 59) 
                    : new DateTime(DateTime.UtcNow.Year, 5, 31, 23, 59, 59) },
                // Legal Studies Refresher
                new SheriffTraining { TrainingTypeId = 1716960, ExpiryDate = new DateTime(DateTime.UtcNow.Year + 1, 12, 31, 23, 59, 59) },
                // Bullying and Harassment (BCSS – In-Service)
                new SheriffTraining { TrainingTypeId = 1716951, ExpiryDate = new DateTime(DateTime.UtcNow.Year + 1, 12, 31, 23, 59, 59) },
                // First Aid (external providers)
                new SheriffTraining { TrainingTypeId = 1716955, ExpiryDate = new DateTime(DateTime.UtcNow.Year + 2, 12, 31, 23, 59, 59) },
                // Critical Incident De-Escalation (Canadian Police Knowledge Network)
                new SheriffTraining { TrainingTypeId = 1718650, ExpiryDate = new DateTime(DateTime.UtcNow.Year + 2, 12, 31, 23, 59, 59) },
                // Fraud Awareness and Prevention (LearningHub)
                new SheriffTraining { TrainingTypeId = 1716956, ExpiryDate = new DateTime(DateTime.UtcNow.Year + 2, 12, 31, 23, 59, 59) },
                // IM 117 (Learning Center)
                new SheriffTraining { TrainingTypeId = 1716958, ExpiryDate = new DateTime(DateTime.UtcNow.Year + 1, 12, 31, 23, 59, 59) },
                // Diversity and Inclusion Essentials (LearningHub)
                new SheriffTraining { TrainingTypeId = 1716954, ExpiryDate = DateTime.UtcNow.AddYears(100) },
                // @Waiting for response from Client: Building Capacity in Indigenous Relations: We are all here to stay (LearningHub)
                // new SheriffTraining { TrainingTypeId = , ExpiryDate = DateTime.UtcNow.AddYears(100) }
            };

            var sheriffTrainings = standardTrainings.Select(standardTraining => new SheriffTraining
            {
                SheriffId = sheriff.Id,
                TrainingTypeId = standardTraining.TrainingTypeId,
                StartDate = DateTimeOffset.UtcNow,
                EndDate = DateTimeOffset.UtcNow,
                ExpiryDate = standardTraining.ExpiryDate,
                Timezone = "UTC"
            }).ToList();

            await Db.SheriffTraining.AddRangeAsync(sheriffTrainings);
            await Db.SaveChangesAsync();
        }

        public async Task<Sheriff> GetSheriff(Guid? id,
            string badgeNumber) =>
            await Db.Sheriff.AsNoTracking()
                .FirstOrDefaultAsync(s =>
                    (badgeNumber == null && id.HasValue && s.Id == id) ||
                    (!id.HasValue && s.BadgeNumber == badgeNumber));

        //Used for the Shift scheduling screen, while considering a location.
        public async Task<List<Sheriff>> GetSheriffsForShiftAvailabilityForLocation(int locationId, DateTimeOffset start, DateTimeOffset end, Guid? sheriffId = null)
        {
            var sheriffQuery = Db.Sheriff.AsNoTracking()
                .AsSplitQuery()
                .Where(s =>
                    (sheriffId == null || sheriffId != null && s.Id == sheriffId) &&
                    s.IsEnabled &&
                    s.HomeLocationId == locationId ||
                    s.AwayLocation.Any(al =>
                        al.LocationId == locationId && !(al.StartDate > end || start > al.EndDate)
                                                    && al.ExpiryDate == null))
                .IncludeSheriffEventsBetweenDates(start, end)
                .IncludeSheriffActingRank();

            return await sheriffQuery.ToListAsync();
        }

        public async Task<List<Sheriff>> GetFilteredSheriffsForTeams()
        {
            var now = DateTimeOffset.UtcNow;
            var sevenDaysFromNow = DateTimeOffset.UtcNow.AddDays(7);

            var sheriffQuery = Db.Sheriff.AsNoTracking()
                .AsSplitQuery()
                .ApplyPermissionFilters(User, now, sevenDaysFromNow, Db)
                .IncludeSheriffEventsBetweenDates(now, sevenDaysFromNow)
                .IncludeSheriffActingRank();

            return await sheriffQuery.ToListAsync();
        }

        public async Task<Sheriff> GetFilteredSheriffForTeams(Guid id)
        {
            var daysPrevious = int.Parse(Configuration.GetNonEmptyValue("DaysInPastToIncludeAwayLocationAndTraining"));
            var minDateForAwayAndTraining = DateTimeOffset.UtcNow.AddDays(-daysPrevious);
            var sevenDaysFromNow = DateTimeOffset.UtcNow.AddDays(7);

            return await Db.Sheriff.AsNoTracking().AsSingleQuery()
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

        public async Task<Sheriff> GetSheriffIdentification(Guid id)
        {
            var daysPrevious = int.Parse(Configuration.GetNonEmptyValue("DaysInPastToIncludeAwayLocationAndTraining"));
            var minDateForAwayAndTraining = DateTimeOffset.UtcNow.AddDays(-daysPrevious);
            var sevenDaysFromNow = DateTimeOffset.UtcNow.AddDays(7);

            return await Db.Sheriff.AsNoTracking()
                .ApplyPermissionFilters(User, minDateForAwayAndTraining, sevenDaysFromNow, Db)
                .Include(s => s.HomeLocation)
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<SheriffLeave>> GetSheriffLeaves(Guid id)
        {
            var daysPrevious = int.Parse(Configuration.GetNonEmptyValue("DaysInPastToIncludeAwayLocationAndTraining"));
            var minDateForAwayAndTraining = DateTimeOffset.UtcNow.AddDays(-daysPrevious);

            return await Db.SheriffLeave.AsNoTracking()
                .Where(l => l.SheriffId == id && l.EndDate >= minDateForAwayAndTraining && l.ExpiryDate == null)
                .Include(l => l.LeaveType)
                .ToListAsync();
        }

        public async Task<List<SheriffAwayLocation>> GetSheriffAwayLocations(Guid id)
        {
            var daysPrevious = int.Parse(Configuration.GetNonEmptyValue("DaysInPastToIncludeAwayLocationAndTraining"));
            var minDateForAwayAndTraining = DateTimeOffset.UtcNow.AddDays(-daysPrevious);

            return await Db.SheriffAwayLocation.AsNoTracking()
                .Where(al => al.SheriffId == id && al.EndDate >= minDateForAwayAndTraining && al.ExpiryDate == null)
                .Include(al => al.Location)
                .ToListAsync();
        }

        public async Task<List<SheriffActingRank>> GetSheriffActingRanks(Guid id)
        {
            return await Db.SheriffActingRank.AsNoTracking()
                .Where(ar => ar.SheriffId == id && ar.ExpiryDate == null)
                .ToListAsync();
        }

        public async Task<List<UserRole>> GetSheriffRoles(Guid id)
        {
            return await Db.UserRole.AsNoTracking()
                .Where(ur => ur.UserId == id)
                .Include(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<List<SheriffTraining>> GetSheriffTrainings(Guid id)
        {
            return await Db.SheriffTraining.AsNoTracking()
                .Where(t => t.SheriffId == id && t.ExpiryDate == null)
                .Include(t => t.TrainingType)
                .ToListAsync();
        }

        public async Task<Sheriff> UpdateSheriff(Sheriff sheriff, bool canEditIdir)
        {
            var savedSheriff = await Db.Sheriff.FindAsync(sheriff.Id);
            savedSheriff.ThrowBusinessExceptionIfNull($"{nameof(Sheriff)} with the id: {sheriff.Id} could not be found. ");

            if (sheriff.BadgeNumber != savedSheriff.BadgeNumber)
                await CheckForBlankOrDuplicateBadgeNumber(sheriff.BadgeNumber);

            if (canEditIdir && savedSheriff.IdirName != sheriff.IdirName)
            {
                await CheckForBlankOrDuplicateIdirName(sheriff.IdirName, sheriff.Id);
                sheriff.IdirName = sheriff.IdirName.ToLower();
                sheriff.IdirId = null;
            }
            else
            {
                sheriff.IdirName = savedSheriff.IdirName;
                sheriff.IdirId = savedSheriff.IdirId;
            }

            Db.Entry(savedSheriff).CurrentValues.SetValues(sheriff);

            Db.Entry(savedSheriff).Property(x => x.HomeLocationId).IsModified = false;
            Db.Entry(savedSheriff).Property(x => x.IsEnabled).IsModified = false;
            Db.Entry(savedSheriff).Property(x => x.Photo).IsModified = false;
            Db.Entry(savedSheriff).Property(x => x.LastPhotoUpdate).IsModified = false;
            Db.Entry(savedSheriff).Property(x => x.KeyCloakId).IsModified = false;
            Db.Entry(savedSheriff).Property(x => x.LastLogin).IsModified = false;

            await Db.SaveChangesAsync();
            return sheriff;
        }

        public async Task<byte[]> GetPhoto(Guid id)
        {
            var savedSheriff = await Db.Sheriff.FindAsync(id);
            savedSheriff.ThrowBusinessExceptionIfNull($"No {nameof(Sheriff)} with Id: {id}");
            return savedSheriff.Photo;
        }

        public async Task<Sheriff> UpdateSheriffPhoto(Guid? id, string badgeNumber, byte[] photoData)
        {
            var savedSheriff = await Db.Sheriff.FirstOrDefaultAsync(s => (id.HasValue && s.Id == id) || (!id.HasValue && s.BadgeNumber == badgeNumber));
            savedSheriff.ThrowBusinessExceptionIfNull($"No {nameof(Sheriff)} with Badge: {badgeNumber} or Id: {id}");
            savedSheriff.Photo = photoData;
            savedSheriff.LastPhotoUpdate = DateTime.UtcNow;
            await Db.SaveChangesAsync();
            return savedSheriff;
        }

        public async Task<Sheriff> UpdateSheriffExcused(Sheriff sheriff)
        {
            var savedSheriff = await Db.Sheriff.FindAsync(sheriff.Id);
            savedSheriff.ThrowBusinessExceptionIfNull($"No {nameof(Sheriff)} with Id: {sheriff.Id}");
            savedSheriff.Excused = sheriff.Excused;
            await Db.SaveChangesAsync();
            return savedSheriff;
        }

        public async Task UpdateSheriffHomeLocation(Guid id, int locationId)
        {
            var savedSheriff = await Db.Sheriff.FindAsync(id);
            savedSheriff.ThrowBusinessExceptionIfNull($"{nameof(Sheriff)} with the id: {id} could not be found. ");
            savedSheriff.HomeLocation = await Db.Location.FindAsync(locationId);
            savedSheriff.HomeLocation.ThrowBusinessExceptionIfNull($"{nameof(Location)} with the id: {locationId} could not be found. ");
            await Db.SaveChangesAsync();
        }

        #endregion Sheriff

        #region Sheriff Event

        public async Task<T> GetSheriffEvent<T>(int id) where T : SheriffEvent =>
            await Db.Set<T>().AsNoTracking().FirstOrDefaultAsync(sal => sal.Id == id);

        #endregion Sheriff Event

        #region Sheriff Location

        public async Task<SheriffAwayLocation> AddSheriffAwayLocation(DutyRosterService dutyRosterService, ShiftService shiftService, SheriffAwayLocation awayLocation, bool overrideConflicts)
        {
            ValidateStartAndEndDates(awayLocation.StartDate, awayLocation.EndDate);
            await ValidateSheriffExists(awayLocation.SheriffId);
            await ValidateNoOverlapAsync(dutyRosterService, shiftService, awayLocation, overrideConflicts);

            awayLocation.Location = await Db.Location.FindAsync(awayLocation.LocationId);
            awayLocation.Sheriff = await Db.Sheriff.FindAsync(awayLocation.SheriffId);
            await Db.SheriffAwayLocation.AddAsync(awayLocation);
            await Db.SaveChangesAsync();
            return awayLocation;
        }

        public async Task<SheriffAwayLocation> UpdateSheriffAwayLocation(DutyRosterService dutyRosterService, ShiftService shiftService, SheriffAwayLocation awayLocation, bool overrideConflicts)
        {
            ValidateStartAndEndDates(awayLocation.StartDate, awayLocation.EndDate);
            await ValidateSheriffExists(awayLocation.SheriffId);

            var savedAwayLocation = await Db.SheriffAwayLocation.FindAsync(awayLocation.Id);
            savedAwayLocation.ThrowBusinessExceptionIfNull($"{nameof(SheriffAwayLocation)} with the id: {awayLocation.Id} could not be found. ");

            if (savedAwayLocation.ExpiryDate.HasValue)
                throw new BusinessLayerException($"{nameof(SheriffAwayLocation)} with the id: {awayLocation.Id} has been expired");

            await ValidateNoOverlapAsync(dutyRosterService, shiftService, awayLocation, overrideConflicts, awayLocation.Id);

            Db.Entry(savedAwayLocation).CurrentValues.SetValues(awayLocation);
            Db.Entry(savedAwayLocation).Property(x => x.SheriffId).IsModified = false;
            Db.Entry(savedAwayLocation).Property(x => x.ExpiryDate).IsModified = false;
            Db.Entry(savedAwayLocation).Property(x => x.ExpiryReason).IsModified = false;
            await Db.SaveChangesAsync();
            return savedAwayLocation;
        }

        public async Task RemoveSheriffAwayLocation(int id, string expiryReason)
        {
            var sheriffAwayLocation = await Db.SheriffAwayLocation.FindAsync(id);
            sheriffAwayLocation.ThrowBusinessExceptionIfNull(
                $"{nameof(SheriffAwayLocation)} with the id: {id} could not be found. ");
            sheriffAwayLocation.ExpiryDate = DateTimeOffset.UtcNow;
            sheriffAwayLocation.ExpiryReason = expiryReason;
            await Db.SaveChangesAsync();
        }

        #endregion Sheriff Location

        #region Sheriff Acting Rank

        public async Task<SheriffActingRank> AddSheriffActingRank(DutyRosterService dutyRosterService, ShiftService shiftService, SheriffActingRank actingRank, bool overrideConflicts)
        {
            ValidateStartAndEndDates(actingRank.StartDate, actingRank.EndDate);
            await ValidateSheriffExists(actingRank.SheriffId);

            await ValidateSheriffActingRankExists(actingRank);

            actingRank.Sheriff = await Db.Sheriff.FindAsync(actingRank.SheriffId);
            await Db.SheriffActingRank.AddAsync(actingRank);
            await Db.SaveChangesAsync();
            return actingRank;
        }

        public async Task<SheriffActingRank> UpdateSheriffActingRank(DutyRosterService dutyRosterService, ShiftService shiftService, SheriffActingRank actingRank, bool overrideConflicts)
        {
            ValidateStartAndEndDates(actingRank.StartDate, actingRank.EndDate);
            await ValidateSheriffExists(actingRank.SheriffId);

            var savedActingRank = await Db.SheriffActingRank.FindAsync(actingRank.Id);
            savedActingRank.ThrowBusinessExceptionIfNull($"{nameof(SheriffActingRank)} with the id: {actingRank.Id} could not be found. ");

            if (savedActingRank.ExpiryDate.HasValue)
                throw new BusinessLayerException($"{nameof(SheriffActingRank)} with the id: {actingRank.Id} has been expired");

            await ValidateSheriffActingRankExists(actingRank);

            Db.Entry(savedActingRank).CurrentValues.SetValues(actingRank);
            Db.Entry(savedActingRank).Property(x => x.SheriffId).IsModified = false;
            Db.Entry(savedActingRank).Property(x => x.ExpiryDate).IsModified = false;
            Db.Entry(savedActingRank).Property(x => x.ExpiryReason).IsModified = false;
            await Db.SaveChangesAsync();
            return savedActingRank;
        }

        public async Task RemoveSheriffActingRank(int id, string expiryReason)
        {
            var sheriffActingRank = await Db.SheriffActingRank.FindAsync(id);
            sheriffActingRank.ThrowBusinessExceptionIfNull(
                $"{nameof(SheriffActingRank)} with the id: {id} could not be found. ");
            sheriffActingRank.ExpiryDate = DateTimeOffset.UtcNow;
            sheriffActingRank.ExpiryReason = expiryReason;
            await Db.SaveChangesAsync();
        }

        #endregion Sheriff Acting Rank

        #region Sheriff Leave

        public async Task<SheriffLeave> AddSheriffLeave(DutyRosterService dutyRosterService, ShiftService shiftService, SheriffLeave sheriffLeave, bool overrideConflicts)
        {
            ValidateStartAndEndDates(sheriffLeave.StartDate, sheriffLeave.EndDate);
            await ValidateSheriffExists(sheriffLeave.SheriffId);
            await ValidateNoOverlapAsync(dutyRosterService, shiftService, sheriffLeave, overrideConflicts);

            sheriffLeave.LeaveType = await Db.LookupCode.FindAsync(sheriffLeave.LeaveTypeId);
            sheriffLeave.Sheriff = await Db.Sheriff.FindAsync(sheriffLeave.SheriffId);
            await Db.SheriffLeave.AddAsync(sheriffLeave);
            await Db.SaveChangesAsync();
            return sheriffLeave;
        }

        public async Task<SheriffLeave> UpdateSheriffLeave(DutyRosterService dutyRosterService, ShiftService shiftService, SheriffLeave sheriffLeave, bool overrideConflicts)
        {
            ValidateStartAndEndDates(sheriffLeave.StartDate, sheriffLeave.EndDate);
            await ValidateSheriffExists(sheriffLeave.SheriffId);

            var savedLeave = await Db.SheriffLeave.FindAsync(sheriffLeave.Id);
            savedLeave.ThrowBusinessExceptionIfNull(
                $"{nameof(SheriffLeave)} with the id: {sheriffLeave.Id} could not be found. ");

            if (savedLeave.ExpiryDate.HasValue)
                throw new BusinessLayerException($"{nameof(SheriffLeave)} with the id: {sheriffLeave.Id} has been expired");

            await ValidateNoOverlapAsync(dutyRosterService, shiftService, sheriffLeave, overrideConflicts, sheriffLeave.Id);

            Db.Entry(savedLeave).CurrentValues.SetValues(sheriffLeave);
            Db.Entry(savedLeave).Property(x => x.SheriffId).IsModified = false;
            Db.Entry(savedLeave).Property(x => x.ExpiryDate).IsModified = false;
            Db.Entry(savedLeave).Property(x => x.ExpiryReason).IsModified = false;

            await Db.SaveChangesAsync();
            return savedLeave;
        }

        public async Task RemoveSheriffLeave(int id, string expiryReason)
        {
            var sheriffLeave = await Db.SheriffLeave.FindAsync(id);
            sheriffLeave.ThrowBusinessExceptionIfNull(
                $"{nameof(SheriffLeave)} with the id: {sheriffLeave.Id} could not be found. ");
            sheriffLeave.ExpiryDate = DateTimeOffset.UtcNow;
            sheriffLeave.ExpiryReason = expiryReason;
            await Db.SaveChangesAsync();
        }

        #endregion Sheriff Leave

        #region Sheriff Training

        public async Task<List<Sheriff>> GetSheriffsTraining()
        {
            var daysPrevious = int.Parse(Configuration.GetNonEmptyValue("DaysInPastToIncludeAwayLocationAndTraining"));
            var minDateForAwayAndTraining = DateTimeOffset.UtcNow.AddDays(-daysPrevious);
            var sevenDaysFromNow = DateTimeOffset.UtcNow.AddDays(7);

            var sheriffQuery = Db.Sheriff.AsNoTracking()
                .AsSplitQuery()
                .ApplyPermissionFilters(User, minDateForAwayAndTraining, sevenDaysFromNow, Db)
                .Include(s => s.Training.Where(t => t.ExpiryDate == null))
                .ThenInclude(t => t.TrainingType)
                .Where(t => t.Training.Count > 0);

            return await sheriffQuery.ToListAsync();
        }

        public async Task<SheriffTraining> AddSheriffTraining(DutyRosterService dutyRosterService, ShiftService shiftService, SheriffTraining sheriffTraining, bool overrideConflicts, bool allowConflictingEvents)
        {
            ValidateStartAndEndDates(sheriffTraining.StartDate, sheriffTraining.EndDate);
            await ValidateSheriffExists(sheriffTraining.SheriffId);
            if (!allowConflictingEvents) await ValidateNoOverlapAsync(dutyRosterService, shiftService, sheriffTraining, overrideConflicts);

            sheriffTraining.Sheriff = await Db.Sheriff.FindAsync(sheriffTraining.SheriffId);
            sheriffTraining.TrainingType = await Db.LookupCode.FindAsync(sheriffTraining.TrainingTypeId);
            await Db.SheriffTraining.AddAsync(sheriffTraining);
            await Db.SaveChangesAsync();
            return sheriffTraining;
        }

        public async Task<SheriffTraining> UpdateSheriffTraining(DutyRosterService dutyRosterService, ShiftService shiftService, SheriffTraining sheriffTraining, bool overrideConflicts, bool allowConflictingEvents)
        {
            ValidateStartAndEndDates(sheriffTraining.StartDate, sheriffTraining.EndDate);
            await ValidateSheriffExists(sheriffTraining.SheriffId);

            var savedTraining = await Db.SheriffTraining.FindAsync(sheriffTraining.Id);
            savedTraining.ThrowBusinessExceptionIfNull(
                $"{nameof(SheriffTraining)} with the id: {sheriffTraining.Id} could not be found. ");

            if (savedTraining.ExpiryDate.HasValue)
                throw new BusinessLayerException($"{nameof(SheriffTraining)} with the id: {sheriffTraining.Id} has been expired");

            if (!allowConflictingEvents) await ValidateNoOverlapAsync(dutyRosterService, shiftService, sheriffTraining, overrideConflicts, sheriffTraining.Id);

            Db.Entry(savedTraining).CurrentValues.SetValues(sheriffTraining);
            Db.Entry(savedTraining).Property(x => x.SheriffId).IsModified = false;
            Db.Entry(savedTraining).Property(x => x.ExpiryDate).IsModified = false;
            Db.Entry(savedTraining).Property(x => x.ExpiryReason).IsModified = false;

            await Db.SaveChangesAsync();
            return savedTraining;
        }

        public async Task RemoveSheriffTraining(int id, string expiryReason)
        {
            var sheriffTraining = await Db.SheriffTraining.FindAsync(id);
            sheriffTraining.ThrowBusinessExceptionIfNull(
                $"{nameof(SheriffTraining)} with the id: {id} could not be found. ");
            sheriffTraining.ExpiryDate = DateTimeOffset.UtcNow;
            sheriffTraining.ExpiryReason = expiryReason;
            await Db.SaveChangesAsync();
        }

        #endregion Sheriff Training

        #region Helpers

        #region Validation

        private async Task CheckForBlankOrDuplicateIdirName(string idirName, Guid? excludeSheriffId = null)
        {
            if (string.IsNullOrEmpty(idirName))
                throw new BusinessLayerException($"Missing {nameof(idirName)}.");

            var existingSheriffWithIdir = await Db.Sheriff.FirstOrDefaultAsync(s => s.IdirName.ToLower() == idirName.ToLower() && s.Id != excludeSheriffId);
            if (existingSheriffWithIdir != null)
                throw new BusinessLayerException(
                    $"Sheriff {existingSheriffWithIdir.LastName}, {existingSheriffWithIdir.FirstName} has IDIR name: {existingSheriffWithIdir.IdirName}");
        }

        private async Task CheckForBlankOrDuplicateBadgeNumber(string badgeNumber)
        {
            if (string.IsNullOrEmpty(badgeNumber))
                throw new BusinessLayerException($"Missing {nameof(badgeNumber)}.");

            var existingSheriffWithBadge = await Db.Sheriff.FirstOrDefaultAsync(s => s.BadgeNumber == badgeNumber);
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
            if (!await Db.Sheriff.AsNoTracking().AnyAsync(s => s.Id == sheriffId))
                throw new BusinessLayerException($"Sheriff with id: {sheriffId} does not exist.");
        }

        private async Task ValidateSheriffActingRankExists(SheriffActingRank actingRank)
        {
            var conflictingRanks = await Db.SheriffActingRank.AsNoTracking().Where(sal =>
                     sal.Id != actingRank.Id &&
                     sal.SheriffId == actingRank.SheriffId &&
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

        private async Task ValidateNoOverlapAsync<T>(DutyRosterService dutyRosterService, ShiftService shiftService, T data, bool overrideConflicts, int? updateOnlyId = null) where T : SheriffEvent
        {
            var sheriffEventConflicts = new List<SheriffEvent>
            {
                await LookForSheriffEventConflictAsync(Db.SheriffLeave, data, updateOnlyId),
                await LookForSheriffEventConflictAsync(Db.SheriffTraining, data, updateOnlyId),
                await LookForSheriffEventConflictAsync(Db.SheriffAwayLocation, data, updateOnlyId)
            }.WhereToList(se => se != null);

            var shiftConflicts = await Db.Shift.AsNoTracking().Include(d => d.Location)
                .Where(sal =>
                sal.SheriffId == data.SheriffId && (sal.StartDate < data.EndDate && data.StartDate < sal.EndDate) &&
                sal.ExpiryDate == null).ToListAsync();

            sheriffEventConflicts = AllowDifferentTimezonesCloseTimeGap(sheriffEventConflicts, data);

            if (!overrideConflicts)
                DisplayConflicts(data.SheriffId, sheriffEventConflicts, shiftConflicts);
            else
                await OverrideConflicts(data, dutyRosterService, shiftService, sheriffEventConflicts, shiftConflicts);
        }

        /// <summary>
        /// This is built for the case where we create an away location in -8 for say a day (00:00:00 to 23:59:00 PST),
        /// then immediately the day after schedule training in -7 (00:00:00 to 23:59:00 MST).
        /// The two overlap, because the EndDate for away location in PST is 00:59:00 MST.
        /// This same scenario could happen moving backwards in time. 00:00:00 in -7 is 23:00:00 in -8.
        /// </summary>
        private List<SheriffEvent> AllowDifferentTimezonesCloseTimeGap<T>(List<SheriffEvent> sheriffEventConflicts, T data) where T : SheriffEvent
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

        private void DisplayConflicts(Guid sheriffId, List<SheriffEvent> sheriffEventConflicts, List<Shift> shiftConflicts)
        {
            DateTimeOffset startDate;
            string startDateFormatted;
            string endDateFormatted;
            var conflictErrors = new List<(DateTimeOffset start, string message)>();

            foreach (var eventConflict in sheriffEventConflicts)
            {
                if (eventConflict is SheriffAwayLocation sheriffAwayLocation)
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
                    var homeLocationId = Db.Sheriff.AsNoTracking().FirstOrDefault(s => s.Id == sheriffId)
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

        private async Task OverrideConflicts<T>(T data, DutyRosterService dutyRosterService, ShiftService shiftService, List<SheriffEvent> sheriffEventConflicts, List<Shift> shiftConflicts) where T : SheriffEvent
        {
            foreach (var eventConflict in sheriffEventConflicts)
            {
                switch (eventConflict)
                {
                    case SheriffAwayLocation _:
                        var sheriffAwayLocation = Db.SheriffAwayLocation.First(sl => sl.Id == eventConflict.Id);
                        sheriffAwayLocation.ExpiryDate = DateTimeOffset.UtcNow;
                        sheriffAwayLocation.ExpiryReason = "OVERRIDE";
                        break;

                    case SheriffTraining _:
                        var sheriffTraining = Db.SheriffTraining.First(sl => sl.Id == eventConflict.Id);
                        sheriffTraining.ExpiryDate = DateTimeOffset.UtcNow;
                        sheriffTraining.ExpiryReason = "OVERRIDE";
                        break;

                    case SheriffLeave _:
                        var sheriffLeave = Db.SheriffLeave.First(sl => sl.Id == eventConflict.Id);
                        sheriffLeave.ExpiryDate = DateTimeOffset.UtcNow;
                        sheriffLeave.ExpiryReason = "OVERRIDE";
                        break;
                }
            }

            await shiftService.HandleShiftOverrides(data, dutyRosterService, shiftConflicts);
        }

        #endregion Overlap Detection && Override

        private async Task<T1> LookForSheriffEventConflictAsync<T1, T2>(DbSet<T1> dbSet, T2 data, int? updateOnlyId = null) where T1 : SheriffEvent where T2 : SheriffEvent
        {
            return await dbSet.AsNoTracking().FirstOrDefaultAsync(sal =>
                sal.SheriffId == data.SheriffId && (sal.StartDate < data.EndDate && data.StartDate < sal.EndDate) &&
                sal.ExpiryDate == null &&
                (typeof(T1) != typeof(T2) ||
                (typeof(T1) == typeof(T2) && (!updateOnlyId.HasValue || updateOnlyId.HasValue && sal.Id != updateOnlyId))));
        }

        #endregion Validation

        #endregion Helpers
    }
}