using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SilvermineNordic.Repository;
using SilvermineNordic.Repository.Services;

[assembly: FunctionsStartup(typeof(SilvermineNordic.Functions.Startup))]
namespace SilvermineNordic.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var executionContextOptions = builder.Services.BuildServiceProvider()
                .GetRequiredService<IOptions<ExecutionContextOptions>>().Value;

            var config = new ConfigurationBuilder()
                .SetBasePath(executionContextOptions.AppDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            string snowMakingStorageConnectionString = config.GetConnectionStringOrSetting("SnowMakingStorageConnectionString");
            string snowMakingStorageName = config.GetConnectionStringOrSetting("SnowMakingStorageName");
            string snowMakingSqlConnectionString = config.GetConnectionStringOrSetting("SnowMakingSqlConnectionString");

            builder.Services.AddSingleton<Repository.IConfiguration>(_ => 
                new ConfigurationService(storageConnectionString: snowMakingStorageConnectionString,
                    storageName: snowMakingStorageName,
                    sqlConnectionString: snowMakingSqlConnectionString));

            //ISilvermineNordicDbContextOptionsFactory dbContextOptionsfactory =
            //    new SilvermineNordicDbContextOptionsFactory(snowMakingSqlConnectionString, DbContextTypeEnum.SqlServer);

            builder.Services.AddDbContext<SilvermineNordicDbContext>();

            builder.Services.AddScoped<IRepositorySensorReading, EntityFrameworkSensorReadingService>();
            builder.Services.AddScoped<IRepositoryThreshold, EntityFrameworkThresholdService>();
        }
    }
}
