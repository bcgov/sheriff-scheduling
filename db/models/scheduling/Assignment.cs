using System;
using System.ComponentModel.DataAnnotations;
using db.models;
using Mapster;
using CAS.API.Models.DB;
using CAS.COMMON.attributes.mapping;
using CAS.COMMON.helpers.extensions;
using CAS.DB.models;

namespace CAS.DB.models.scheduling
{
    [AdaptTo("[name]Dto")]
    [GenerateUpdateDto, GenerateAddDto]
    public class Assignment : BaseEntity
    {
        [Key]
        [ExcludeFromAddDto]
        public int Id { get; set; }
        [ExcludeFromAddAndUpdateDto]
        public LookupCode LookupCode { get; set; } 
        public int LookupCodeId { get; set; }
        public DateTimeOffset? AdhocStartDate { get; set; }
        public DateTimeOffset? AdhocEndDate { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public string Timezone { get; set; }
        public string Name { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        [ExcludeFromAddAndUpdateDto]
        public Location Location { get; set; }
        public int LocationId { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string ExpiryReason { get; set; }
        [MaxLength(200)]
        public string Comment { get; set; }
        public bool IsAvailableOnDate(DateTimeOffset date)
        {
            return date.DayOfWeek switch
            {
                DayOfWeek.Monday => Monday,
                DayOfWeek.Tuesday => Tuesday,
                DayOfWeek.Wednesday => Wednesday,
                DayOfWeek.Thursday => Thursday,
                DayOfWeek.Friday => Friday,
                DayOfWeek.Saturday => Saturday,
                DayOfWeek.Sunday => Sunday,
                _ => false
            };
        }

        /// <summary>
        /// This filters by adhoc dates - if it's not adhoc, filters by day of week.
        /// Ideally, we'd like to pass in TZ adjusted dates. (DateTimeOffset adjusted to the correct TZ)
        /// This needs to convert the provided dates to their TZ dates.
        /// For example: November 6th - 01:00:00 UTC time, Is November 5th in the PST/PDT timezone.  
        /// </summary>
        public bool HasAtLeastOneDayOverlap(DateTimeOffset? start, DateTimeOffset? end)
        {
            if (!start.HasValue || !end.HasValue)  return true;
            if (start > end)  return false;

            var startInTz = start.Value.ConvertToTimezone(Timezone);
            var endInTz = end.Value.ConvertToTimezone(Timezone);

            if (startInTz == endInTz)  
                return IsAvailableOnDate(startInTz);

            if (AdhocStartDate.HasValue && AdhocEndDate.HasValue &&
                (AdhocStartDate > endInTz || startInTz > AdhocEndDate) )
                return false;

            var dt = startInTz;
            while (dt < endInTz)
            {
                switch (dt.DayOfWeek)
                {
                    case DayOfWeek.Monday when Monday:
                    case DayOfWeek.Tuesday when Tuesday:
                    case DayOfWeek.Wednesday when Wednesday:
                    case DayOfWeek.Thursday when Thursday:
                    case DayOfWeek.Friday when Friday:
                    case DayOfWeek.Saturday when Saturday:
                    case DayOfWeek.Sunday when Sunday:
                        return true;
                    default:
                        break;
                }
                dt = dt.TranslateDateForDaylightSavings(Timezone, 1);
            }
            return false;
        }
    }
}
