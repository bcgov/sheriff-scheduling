using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using CAS.API.helpers;
using CAS.API.models.ches;
using CAS.API.services;
using tests.api.Helpers;
using Xunit;

namespace tests.services
{
    public class ChesEmailServiceTests
    {
        [Fact(Skip = "Adhoc test.")]
        public async Task SendTestEmail()
        {
            var env = new EnvironmentBuilder(null,null,null);

            var options =
                EnvironmentBuilder.CreateIOptionSnapshotMock(env.Configuration.GetSection(ChesEmailOptions.Position)
                    .Get<ChesEmailOptions>());

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

            var chesService = new ChesEmailService(options, httpClientFactory.Object, env.LogFactory.CreateLogger<ChesEmailService>());

            await chesService.SendEmail("Hello", "Test",
                env.Configuration.GetNonEmptyValue("TestEmailAddress"));
        }
    }
}
