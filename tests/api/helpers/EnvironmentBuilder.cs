using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using CAS.API.helpers;
using CAS.API.helpers.extensions;
using CAS.DB.models;
using BasicAuthenticationHeaderValue = CAS.API.helpers.BasicAuthenticationHeaderValue;

namespace tests.api.Helpers
{
    /// <summary>
    /// This builds out our environment before injecting into controllers.
    /// </summary>
    public class EnvironmentBuilder
    {
        public ILoggerFactory LogFactory;
        public HttpClient HttpClient;
        public IConfiguration Configuration;

        public EnvironmentBuilder(string usernameKey, string passwordKey, string urlKey)
        {
            //Load in secrets, this uses the secrets file as the API.
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: true);
            builder.AddUserSecrets<EnvironmentBuilder>();
            Configuration = builder.Build();

            //Create HTTP client, usually done by Startup.cs - which handles the life cycle of HttpClient nicely.
            HttpClient = new HttpClient();

            if (usernameKey != null && passwordKey != null && urlKey != null)
            {
                HttpClient.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(
                    Configuration.GetNonEmptyValue(usernameKey),
                    Configuration.GetNonEmptyValue(passwordKey));

                HttpClient.BaseAddress = new Uri(Configuration.GetNonEmptyValue(urlKey).EnsureEndingForwardSlash());
            }

            //Create logger.
            LogFactory = LoggerFactory.Create(loggingBuilder =>
            {
                loggingBuilder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });
        }

        public static DbContextOptions<CourtAdminDbContext> SetupDbOptions(bool useMemoryDatabase = false)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: true);
            builder.AddUserSecrets<EnvironmentBuilder>();
            var configuration = builder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<CourtAdminDbContext>();
            if (useMemoryDatabase)
                optionsBuilder.UseInMemoryDatabase("testingDb");
            else
                optionsBuilder.UseNpgsql(configuration.GetNonEmptyValue("DatabaseConnectionString")).EnableSensitiveDataLogging(true);

            return optionsBuilder.Options;
        }

        public static IOptionsSnapshot<T> CreateIOptionSnapshotMock<T>(T value) where T : class, new()
        {
            var mock = new Mock<IOptionsSnapshot<T>>();
            mock.Setup(m => m.Value).Returns(value);
            return mock.Object;
        }

    }

}