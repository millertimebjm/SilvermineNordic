using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            string snowMakingSqlConnectionString = config.GetConnectionStringOrSetting("SnowMakingSqlConnectionString");
            string azureSmsConnectionString = config.GetConnectionStringOrSetting("AzureSmsConnectionString");
            string azureSmsFromPhone = config.GetConnectionStringOrSetting("AzureSmsFromPhone");
            string openWeatherApiForecastApiKey = config.GetConnectionStringOrSetting("OpenWeatherApiForecastApiKey");
            string zoneNotificationPhoneNumbers = config.GetConnectionStringOrSetting("ZoneNotificationPhoneNumbers");

            builder.Services.AddSingleton<ISilvermineNordicConfiguration>(_ =>
                new SilvermineNordicConfigurationService()
                {
                    SqlConnectionString = snowMakingSqlConnectionString,
                    OpenWeatherApiKey = openWeatherApiForecastApiKey,
                    AzureSmsConnectionString = azureSmsConnectionString,
                    AzureSmsFromPhone = azureSmsFromPhone,
                    ZoneNotificationPhoneNumbers = zoneNotificationPhoneNumbers,
                });

            builder.Services.AddDbContext<SilvermineNordicDbContext>();
            builder.Services.AddScoped<IRepositoryReading, EntityFrameworkReadingService>();
            builder.Services.AddScoped<IRepositoryThreshold, EntityFrameworkThresholdService>();
            builder.Services.AddSingleton<ISms, AzureSmsService>();
            builder.Services.AddSingleton<IWeatherForecast, OpenWeatherApiForecastService>();
        }
    }
}
