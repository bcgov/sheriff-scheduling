﻿using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SS.Db.models.sheriff;

namespace SS.Api.helpers.extensions
{
    public static class SheriffExtensions
    {
        //Include AwayLocation/Training/Leave that is within a date range.
        public static IQueryable<Sheriff> IncludeSheriffEventsBetweenDates(this IQueryable<Sheriff> query, DateTimeOffset startDate, DateTimeOffset endDate)
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

        public static IQueryable<Sheriff> IncludeSheriffActingRank(this IQueryable<Sheriff> query)
        {
            var startDate = DateTimeOffset.UtcNow;
            return query.Include(s => s.ActingRank.Where(ar =>
                    (ar.StartDate <= startDate && startDate < ar.EndDate)
                    && ar.ExpiryDate == null));
        }
    }
}