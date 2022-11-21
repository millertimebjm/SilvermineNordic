using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
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
            // local.settings.json
            //{
            //  "IsEncrypted": false,
            //  "Values": {
            //    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
            //    "FUNCTIONS_WORKER_RUNTIME": "dotnet"
            //  },
            //  "ConnectionStrings": {
            //    "SnowMakingSqlConnectionString": "",
            //    "AzureSmsConnectionString": "",
            //    "AzureSmsFromPhone": "+",
            //    "OpenWeatherApiForecastApiKey": ""
            //  }
            //}
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
            string azureSmsConnectionString = config.GetConnectionStringOrSetting("AzureSmsConnectionString");
            string azureSmsFromPhone = config.GetConnectionStringOrSetting("AzureSmsFromPhone");
            string openWeatherApiForecastApiKey = config.GetConnectionStringOrSetting("OpenWeatherApiForecastApiKey");

            builder.Services.AddSingleton<Repository.IConfiguration>(_ =>
                new ConfigurationService()
                {
                    StorageConnectionString = snowMakingStorageConnectionString,
                    StorageName = snowMakingStorageName,
                    SqlConnectionString = snowMakingSqlConnectionString,
                    OpenWeatherApiKey = openWeatherApiForecastApiKey,
                    AzureSmsConnectionString = azureSmsConnectionString,
                    AzureSmsFromPhone = azureSmsFromPhone,
                });

            builder.Services.AddDbContext<SilvermineNordicDbContext>();
            builder.Services.AddScoped<IRepositorySensorReading, EntityFrameworkSensorReadingService>();
            builder.Services.AddScoped<IRepositoryThreshold, EntityFrameworkThresholdService>();
            builder.Services.AddSingleton<ISms, AzureSmsService>();
            builder.Services.AddSingleton<IWeatherForecast, OpenWeatherApiForecastService>();
        }
    }
}
