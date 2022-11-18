using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SilvermineNordic.Repository.Services;

[assembly: FunctionsStartup(typeof(SilvermineNordic.Functions.Startup))]
namespace SilvermineNordic.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //FunctionsHostBuilderContext context = builder.GetContext();
            var config = new ConfigurationBuilder()
                //.SetBasePath(context.ApplicationRootPath)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            string snowMakingStorageConnectionString = config.GetConnectionStringOrSetting("SnowMakingStorageConnectionString");
            string snowMakingStorageName = config.GetConnectionStringOrSetting("SnowMakingStorageName");
            string snowMakingSqlConnectionString = config.GetConnectionStringOrSetting("SnowMakingSqlConnectionString");

            builder.Services.AddSingleton<IConfiguration>(_ => 
                new ConfigurationService(storageConnectionString: snowMakingStorageConnectionString,
                    storageName: snowMakingStorageName,
                    sqlConnectionString: snowMakingSqlConnectionString));

            //builder.Services.AddDbContextPool<SilvermineNordicDbContext>(
            //    //options => options.UseInMemoryDatabase("SilverminNordicConnectionString"));
            //    options => options.UseSqlServer(snowMakingSqlConnectionString));

            //builder.Services.AddScoped<IRepositorySensorReading, EntityFrameworkSensorReadingService>();
            //builder.Services.AddScoped<IRepositoryThreshold, EntityFrameworkThresholdService>();
        }
    }
}
