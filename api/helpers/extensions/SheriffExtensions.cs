using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CAS.DB.models.courtAdmin;

namespace CAS.API.helpers.extensions
{
    public static class SheriffExtensions
    {
        //Include AwayLocation/Training/Leave that is within a date range.
        public static IQueryable<CourtAdmin> IncludeCourtAdminEventsBetweenDates(this IQueryable<CourtAdmin> query, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return query.Include(s => s.AwayLocation.Where(al =>
                    (al.StartDate < endDate && startDate < al.EndDate)
                    && al.ExpiryDate == null))
                .ThenInclude(al => al.Location)
                .Include(s => s.Training.Where(al =>
                    (al.StartDate < endDate && startDate < al.EndDate)
                    && al.ExpiryDate == null))
                .ThenInclude(t => t.TrainingType)
                .Include(s => s.Leave.Where(al =>
                    (al.StartDate < endDate && startDate < al.EndDate)
                    && al.ExpiryDate == null))
                .ThenInclude(l => l.LeaveType)
                .Include(s => s.HomeLocation);
        }

        public static IQueryable<CourtAdmin> IncludeCourtAdminActingRank(this IQueryable<CourtAdmin> query)
        {
            var startDate = DateTimeOffset.UtcNow;
            return query.Include(s => s.ActingRank.Where(ar =>
                    (ar.StartDate <= startDate && startDate < ar.EndDate)
                    && ar.ExpiryDate == null));
        }
    }
}