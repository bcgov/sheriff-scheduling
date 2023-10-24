using System;
using CAS.COMMON.helpers.extensions;
using Xunit;

namespace tests
{
    public class DaylightSavingsTests
    {
        [Fact]
        public void VancouverDawsonEdmontonCrestonTimeTests()
        {
            //For current daylight savings rules, this test could change in the future. 
            var vancouverTimeZone = "America/Vancouver";
            var dawsonCreekTimeZone = "America/Dawson_Creek";
            var edmontonTimeZone = "America/Edmonton";
            var crestonTimeZone = "America/Creston";

            //Vancouver, Edmonton
            //Sunday March 8th, 2 am -1 
            //Sunday Nov 1st, 2 am +1

            var beforeMarchChange = new DateTimeOffset(2020, 03, 7, 0, 0, 0, new TimeSpan());
            //Vancouver, Edmonton - Crossing March 8th, 2 am -1. Should shift. 
            var adjustedDateTime = beforeMarchChange.TranslateDateForDaylightSavings(vancouverTimeZone, 5);
            Assert.Equal(120 - 1, adjustedDateTime.Subtract(beforeMarchChange).TotalHours);
            adjustedDateTime = beforeMarchChange.TranslateDateForDaylightSavings(edmontonTimeZone, 5);
            Assert.Equal(120 - 1, adjustedDateTime.Subtract(beforeMarchChange).TotalHours); 
            
            // Creston, Dawson Creek - Crossing March 8th, 2 am -1. NO shift.
            adjustedDateTime = beforeMarchChange.TranslateDateForDaylightSavings(dawsonCreekTimeZone, 5);
            Assert.Equal(120, adjustedDateTime.Subtract(beforeMarchChange).TotalHours);
            adjustedDateTime = beforeMarchChange.TranslateDateForDaylightSavings(crestonTimeZone, 5);
            Assert.Equal(120, adjustedDateTime.Subtract(beforeMarchChange).TotalHours);

            var beforeNovemberChange = new DateTimeOffset(2020, 10, 31, 0, 0, 0, new TimeSpan());

            //Vancouver, Edmonton - Crossing Nov 1st, 2 am +1. Should shift.
            adjustedDateTime = beforeNovemberChange.TranslateDateForDaylightSavings(vancouverTimeZone, 5);
            Assert.Equal(120 + 1, adjustedDateTime.Subtract(beforeNovemberChange).TotalHours);
            adjustedDateTime = beforeNovemberChange.TranslateDateForDaylightSavings(edmontonTimeZone, 5);
            Assert.Equal(120 + 1 , adjustedDateTime.Subtract(beforeNovemberChange).TotalHours);

            //Creston, Dawson Creek - Crossing Nov 1st, 2 am +1. NO shift.
            adjustedDateTime = beforeNovemberChange.TranslateDateForDaylightSavings(dawsonCreekTimeZone, 5);
            Assert.Equal(120, adjustedDateTime.Subtract(beforeNovemberChange).TotalHours);
            adjustedDateTime = beforeNovemberChange.TranslateDateForDaylightSavings(crestonTimeZone, 5);
            Assert.Equal(120, adjustedDateTime.Subtract(beforeNovemberChange).TotalHours);
        }
    }
}
