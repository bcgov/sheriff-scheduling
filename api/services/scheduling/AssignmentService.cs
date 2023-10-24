using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CAS.API.helpers.extensions;
using CAS.COMMON.helpers.extensions;
using CAS.DB.models;
using CAS.DB.models.scheduling;

namespace CAS.API.services.scheduling
{
    public class AssignmentService
    {
        private CourtAdminDbContext Db { get; }
        public AssignmentService(CourtAdminDbContext db)
        {
            Db = db;
        }

        public async Task<List<Assignment>> GetAssignments(int locationId, DateTimeOffset? start, DateTimeOffset? end)
        {
            //Order by LookupCodeType, SortOrder, then lack of SortOrder. 
            var assignment = await Db.Assignment.AsSplitQuery().AsNoTracking()
                .Include(a => a.Location)
                .Include(a => a.LookupCode)
                .ThenInclude(lc => lc.SortOrder.Where(so => so.LocationId == locationId))
                .Where(a => a.LocationId == locationId && (a.ExpiryDate == null || a.ExpiryDate > start))
                .OrderBy(a => (int)a.LookupCode.Type)
                .ThenBy(a => a.LookupCode.SortOrder.First().SortOrder)
                .ThenBy(a => a.LookupCodeId)
                .ToListAsync();

            //Ensure we include assignments that have duties within this time range. 
            var assignmentsWithDuties = await Db.Duty.AsNoTracking()
                .Include(d => d.Assignment)
                .Where(d => d.LocationId == locationId &&
                            d.StartDate < end &&
                            start < d.EndDate &&
                            d.ExpiryDate == null &&
                            d.AssignmentId != null)
                .Select(d => d.AssignmentId)
                .ToListAsync();

            //Filter out the date ranges outside of the database. 
            var filteredAssignments = assignment.WhereToList(a => assignmentsWithDuties.Any(ad => ad == a.Id) || a.HasAtLeastOneDayOverlap(start, end));
            filteredAssignments.ForEach(lc => lc.LookupCode.SortOrderForLocation = lc.LookupCode.SortOrder.FirstOrDefault());
            return filteredAssignments;
        }

        public async Task<Assignment> GetAssignment(int id) =>
            await Db.Assignment.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

        public async Task<Assignment> CreateAssignment(Assignment assignment)
        {
            assignment.Timezone.GetTimezone().ThrowBusinessExceptionIfNull($"A valid {nameof(assignment.Timezone)} needs to be included in the assignment.");
            assignment.Location = await Db.Location.FindAsync(assignment.LocationId);
            assignment.LookupCode = await Db.LookupCode.FindAsync(assignment.LookupCodeId);
            await Db.Assignment.AddAsync(assignment);
            await Db.SaveChangesAsync();
            return assignment;
        }

        public async Task<Assignment> UpdateAssignment(Assignment assignment)
        {
            assignment.Timezone.GetTimezone().ThrowBusinessExceptionIfNull($"A valid {nameof(assignment.Timezone)} needs to be included in the assignment.");
            var savedAssignment = await Db.Assignment.FindAsync(assignment.Id);
            savedAssignment.ThrowBusinessExceptionIfNull($"{nameof(Assignment)} with the id: {assignment.Id} could not be found.");

            Db.Entry(savedAssignment).CurrentValues.SetValues(assignment);
            Db.Entry(savedAssignment).Property(a => a.LocationId).IsModified = false;
            Db.Entry(savedAssignment).Property(a => a.ExpiryDate).IsModified = false;
            Db.Entry(savedAssignment).Property(a => a.ExpiryReason).IsModified = false;

            await Db.SaveChangesAsync();
            return savedAssignment;
        }

        public async Task ExpireAssignment(int id, string expiryReason, DateTimeOffset? expiryDate)
        {
            var savedAssignment = await Db.Assignment.FindAsync(id);
            var convertedTime = expiryDate?.ConvertToTimezone(savedAssignment.Timezone) ?? DateTimeOffset.UtcNow.ConvertToTimezone(savedAssignment.Timezone);
            savedAssignment.ExpiryDate = convertedTime.Date;
            savedAssignment.ExpiryReason = expiryReason;

            var duties = await Db.Duty.Include(d => d.DutySlots)
                .Where(d => d.AssignmentId == savedAssignment.Id &&
                            d.StartDate >= convertedTime.Date)
                .ToListAsync();

            duties.ForEach(d => d.ExpiryDate = DateTimeOffset.UtcNow);
            duties.SelectMany(d => d.DutySlots).ToList().ForEach(d => d.ExpiryDate = DateTimeOffset.UtcNow);

            await Db.SaveChangesAsync();
        }
    }
}
