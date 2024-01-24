using CAS.API.helpers.extensions;

namespace CAS.API.models.ches
{
    public class ChesEmailOptions
    {
        public const string Position = "ChesEmailService";
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string AuthUrl { get; set; }
        public string EmailUrl { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }

        public void ValidateOptions()
        {
            ClientId.ThrowConfigurationExceptionIfNull($"{Position}:{ClientId}");
            Secret.ThrowConfigurationExceptionIfNull($"{Position}:{Secret}");
            AuthUrl.ThrowConfigurationExceptionIfNull($"{Position}:{AuthUrl}");
            EmailUrl.ThrowConfigurationExceptionIfNull($"{Position}:{EmailUrl}");
            SenderEmail.ThrowConfigurationExceptionIfNull($"{Position}:{SenderEmail}");
            SenderName.ThrowConfigurationExceptionIfNull($"{Position}:{SenderName}");
        }
    }

    public class ChesEmailAttachment
    {
        public string content { get; set; }
        public string contentType { get; set; }
        public string encoding { get; set; }
        public string filename { get; set; }
    }
}