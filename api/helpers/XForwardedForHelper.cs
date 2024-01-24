namespace CAS.API.helpers
{
    public static class XForwardedForHelper
    {
        public static string BuildUrlString(string forwardedHost, string forwardedPort, string baseUrl)
        {
            var portComponent = string.IsNullOrEmpty(forwardedPort) || forwardedPort == "80" || forwardedPort == "443" ? "" : $":{forwardedPort}";
            return $"https://{forwardedHost}{portComponent}{baseUrl}";
        }
    }
}
