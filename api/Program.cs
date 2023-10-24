using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CAS.API.services.ef;

namespace CAS.API
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            //This needs to run before our host, because it may have essential migrations for AddDataProtection to work. 
            var migrationService = host.Services.GetRequiredService<MigrationAndSeedService>();
            migrationService.ExecuteMigrationsAndSeeds();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}