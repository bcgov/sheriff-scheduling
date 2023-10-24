using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CAS.API.helpers.extensions;
using CAS.API.infrastructure.exceptions;
using CAS.API.models.ches;

namespace CAS.API.services
{
    public class ChesEmailService
    {
        private readonly ChesEmailOptions _chesEmailOptions;
        private ILogger<ChesEmailService> Logger { get; }
        private HttpClient HttpClient { get; }

        public ChesEmailService(IOptionsSnapshot<ChesEmailOptions> chesEmailOptions, IHttpClientFactory httpClientFactory, ILogger<ChesEmailService> logger)
        {
            HttpClient = httpClientFactory.CreateClient(nameof(ChesEmailService));
            _chesEmailOptions = chesEmailOptions?.Value;
            _chesEmailOptions.ThrowConfigurationExceptionIfNull($"{ChesEmailOptions.Position}");
            _chesEmailOptions.ValidateOptions();
            Logger = logger;
        }

        public async Task<string> GetEmailServiceToken()
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, _chesEmailOptions.AuthUrl);
                requestMessage.Headers.Authorization = new BasicAuthenticationHeaderValue(
                    _chesEmailOptions.ClientId,
                    _chesEmailOptions.Secret);
                requestMessage.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "grant_type", "client_credentials" } });

                var response = await HttpClient.SendAsync(requestMessage);
                if (!response.IsSuccessStatusCode)
                    throw new BadRequestException($"While getting access_token for email - Received status code: {response.StatusCode}");

                var contents = await response.Content.ReadAsStringAsync();
                var accessToken = JObject.Parse(contents)["access_token"]?.ToString();
                Logger.LogDebug("Received access token successfully.");
                return accessToken;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error happened while trying to get EmailServiceToken.");
            }
            return null;
        }

        public async Task<Boolean> SendEmail(string body, string subject, string recipientEmail)
        {
            body.ThrowIfNullOrEmpty(nameof(body));
            subject.ThrowIfNullOrEmpty(nameof(subject));
            recipientEmail.ThrowIfNullOrEmpty(nameof(recipientEmail));

            var to = recipientEmail.Split(",").ToList();
            var emailServiceToken = await GetEmailServiceToken();
            emailServiceToken.ThrowIfNullOrEmpty(nameof(emailServiceToken));

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, _chesEmailOptions.EmailUrl);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", emailServiceToken);

            Logger.LogDebug($"Attempting to Send email to {recipientEmail}.");
            try
            {
                //https://ches-master-9f0fbe-prod.pathfinder.gov.bc.ca/api/v1/docs#operation/postEmail
                var email = new
                {
                    bcc = new List<string>(),
                    bodyType = "text",
                    body,
                    cc = new List<string>(),
                    delayTS = 0,
                    encoding = "utf-8",
                    from = $@"{_chesEmailOptions.SenderName} <{_chesEmailOptions.SenderEmail}>", //This isn't clear in their documentation.
                    priority = "normal",
                    subject,
                    to,
                    tag = Guid.NewGuid()
                };

                requestMessage.Content =
                    new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");

                var response = await HttpClient.SendAsync(requestMessage);
                var contents = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new BadRequestException($"While sending email - Received status code: {response.StatusCode} : {contents}");

                Logger.LogInformation($"Email sent to {recipientEmail} successfully.");
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error happened while trying to send email.");
                return false;
            }
        }

        public async Task SendEmailWithPdfAttachment(string body, string subject, string from, string recipientEmail, byte[] pdfContent)
        {
            body.ThrowIfNullOrEmpty(nameof(body));
            subject.ThrowIfNullOrEmpty(nameof(subject));
            from.ThrowIfNullOrEmpty(nameof(from));
            recipientEmail.ThrowIfNullOrEmpty(nameof(recipientEmail));
            var attachments = new List<ChesEmailAttachment>();
            attachments.Add(
                new ChesEmailAttachment
                {
                    content = Convert.ToBase64String(pdfContent),
                    contentType = "application/pdf",
                    encoding = "base64",
                    filename = "Shift_Schedule.pdf"
                }
            );

            var to = recipientEmail.Split(",").ToList();
            var emailServiceToken = await GetEmailServiceToken();
            emailServiceToken.ThrowIfNullOrEmpty(nameof(emailServiceToken));

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, _chesEmailOptions.EmailUrl);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", emailServiceToken);

            Logger.LogDebug($"Attempting to Send email to {recipientEmail}.");
            try
            {
                var email = new
                {
                    bcc = new List<string>(),
                    bodyType = "text",
                    body,
                    cc = new List<string>(),
                    delayTS = 0,
                    encoding = "utf-8",
                    from,
                    priority = "normal",
                    subject,
                    to,
                    tag = Guid.NewGuid(),
                    attachments
                };

                requestMessage.Content =
                    new StringContent(JsonConvert.SerializeObject(email), Encoding.UTF8, "application/json");

                var response = await HttpClient.SendAsync(requestMessage);
                var contents = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new BadRequestException($"While sending email - Received status code: {response.StatusCode} : {contents}");

                Logger.LogInformation($"Email sent to {recipientEmail} successfully.");
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error happened while trying to send email.");
            }
        }
    }
}