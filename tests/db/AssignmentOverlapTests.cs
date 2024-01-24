using System;
using CAS.COMMON.helpers.extensions;
using CAS.DB.models.scheduling;
using Xunit;

namespace tests.db
{
    public class AssignmentOverlapTests
    {
        [Fact]
        public void OverlapTests()
        {
            const string timezone = "America/Vancouver";
            var targetDate = DateTimeOffset.UtcNow.ConvertToTimezone(timezone);
            var assignment = new Assignment
            {
                Timezone = "America/Vancouver", 
                Monday = targetDate.DayOfWeek == DayOfWeek.Monday,
                Tuesday = targetDate.DayOfWeek == DayOfWeek.Tuesday,
                Wednesday = targetDate.DayOfWeek == DayOfWeek.Wednesday,
                Thursday = targetDate.DayOfWeek == DayOfWeek.Thursday,
                Friday = targetDate.DayOfWeek == DayOfWeek.Friday,
                Saturday = targetDate.DayOfWeek == DayOfWeek.Saturday,
                Sunday = targetDate.DayOfWeek == DayOfWeek.Sunday
            };

            //No value case
            Assert.True(assignment.HasAtLeastOneDayOverlap(null, null));

            //False case
            Assert.False(assignment.HasAtLeastOneDayOverlap(targetDate.AddDays(5), targetDate.AddDays(7)));

            //Start = End case
            Assert.True(assignment.HasAtLeastOneDayOverlap(targetDate, targetDate));

            //Loop of dates case 
            Assert.True(assignment.HasAtLeastOneDayOverlap(targetDate.TranslateDateForDaylightSavings(timezone,-1), targetDate.TranslateDateForDaylightSavings(timezone, 1)));

            //Adhoc case
            var adhocAssignment = new Assignment { Timezone = "America/Vancouver", AdhocStartDate = targetDate, AdhocEndDate = targetDate.TranslateDateForDaylightSavings(timezone, 5), Monday= true, Tuesday= true, Wednesday = true, Thursday = true, Friday = true, Saturday = true, Sunday = true};
            Assert.True(adhocAssignment.HasAtLeastOneDayOverlap(targetDate, targetDate.TranslateDateForDaylightSavings(timezone, 1)));
            Assert.False(adhocAssignment.HasAtLeastOneDayOverlap(targetDate.TranslateDateForDaylightSavings(timezone, 40), targetDate.TranslateDateForDaylightSavings(timezone, 50)));

            var daylightSavingsAssignment = new Assignment
            {
                Timezone = "America/Vancouver",
                Monday = DateTimeOffset.Parse("2020-11-02").DayOfWeek == DayOfWeek.Monday,
                Tuesday = DateTimeOffset.Parse("2020-11-02").DayOfWeek == DayOfWeek.Tuesday,
                Wednesday = DateTimeOffset.Parse("2020-11-02").DayOfWeek == DayOfWeek.Wednesday,
                Thursday = DateTimeOffset.Parse("2020-11-02").DayOfWeek == DayOfWeek.Thursday,
                Friday = DateTimeOffset.Parse("2020-11-02").DayOfWeek == DayOfWeek.Friday,
                Saturday = DateTimeOffset.Parse("2020-11-02").DayOfWeek == DayOfWeek.Saturday,
                Sunday = DateTimeOffset.Parse("2020-11-02").DayOfWeek == DayOfWeek.Sunday
            };

            //Loop of dates crossing daylight savings
            Assert.True(daylightSavingsAssignment.HasAtLeastOneDayOverlap(DateTimeOffset.Parse("2020-11-01"), DateTimeOffset.Parse("2020-11-03")));


            var daylightSavingsAssignment2 = new Assignment
            {
                Timezone = "America/Vancouver",
                Monday = DateTimeOffset.Parse("2020-03-08").DayOfWeek == DayOfWeek.Monday,
                Tuesday = DateTimeOffset.Parse("2020-03-08").DayOfWeek == DayOfWeek.Tuesday,
                Wednesday = DateTimeOffset.Parse("2020-03-08").DayOfWeek == DayOfWeek.Wednesday,
                Thursday = DateTimeOffset.Parse("2020-03-08").DayOfWeek == DayOfWeek.Thursday,
                Friday = DateTimeOffset.Parse("2020-03-08").DayOfWeek == DayOfWeek.Friday,
                Saturday = DateTimeOffset.Parse("2020-03-08").DayOfWeek == DayOfWeek.Saturday,
                Sunday = DateTimeOffset.Parse("2020-03-08").DayOfWeek == DayOfWeek.Sunday
            };

            //Loop of dates crossing daylight savings
            Assert.True(daylightSavingsAssignment2.HasAtLeastOneDayOverlap(DateTimeOffset.Parse("2020-03-07"), DateTimeOffset.Parse("2020-03-09")));
        }
    }
}
