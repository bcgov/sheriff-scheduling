using System.Text.RegularExpressions;
using NodaTime;

namespace CAS.API.helpers.extensions
{
    public static class StringExtensions
    {
        public static string EnsureEndingForwardSlash(this string target) => target.EndsWith("/") ? target : $"{target}/";

        public static string ConvertCamelCaseToMultiWord(this string target) =>
            Regex.Replace(target, "([A-Z])", " $1").Trim().ToLower();

        public static DateTimeZone GetTimezone(this string timezone)
        {
            return string.IsNullOrEmpty(timezone) ? null : DateTimeZoneProviders.Tzdb.GetZoneOrNull(timezone);
        }
    }
}
