using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CAS.API.helpers;

namespace CAS.API.services.jc
{
    internal class TimedDataUpdaterService : IHostedService, IDisposable
    {
        private ILogger Logger { get; }
        private Timer _timer;
        public IServiceProvider Services { get; }
        private readonly TimeSpan _checkForUpdate;


        public TimedDataUpdaterService(IServiceProvider services, ILogger<TimedDataUpdaterService> logger, IConfiguration configuration)
        {
            Services = services;
            Logger = logger;
            _checkForUpdate = TimeSpan.Parse(configuration.GetNonEmptyValue("JCSynchronization:CheckForUpdate"));

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Timed Background Service is starting with a period of {_checkForUpdate}.");
            _timer = new Timer(DoWorkWrapper, null, new TimeSpan(), _checkForUpdate);
            return Task.CompletedTask;
        }

        private void DoWorkWrapper(object state)
        {
            _ = DoWork();
        }

        private async Task DoWork()
        {
            try
            {
                Logger.LogInformation("Timed Background Service is working.");

                using var scope = Services.CreateScope();
                var justinDataUpdaterService =
                    scope.ServiceProvider
                        .GetRequiredService<JCDataUpdaterService>();

                if (!await justinDataUpdaterService.ShouldSynchronize()) return;

                Logger.LogInformation("Syncing Regions.");
                await justinDataUpdaterService.SyncRegions();
                Logger.LogInformation("Syncing Locations.");
                await justinDataUpdaterService.SyncLocations();
                Logger.LogInformation("Syncing CourtRooms.");
                await justinDataUpdaterService.SyncCourtRooms();
                Logger.LogInformation("Finished Syncing.");
            }
            catch (Exception e)
            {
                Logger.LogError(e, "An error happened while syncing regions/locations/courtrooms.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
