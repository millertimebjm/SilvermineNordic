using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SilvermineNordic.Repository.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SilvermineNordic.Repository;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

//var config = new ConfigurationBuilder()
//                .SetBasePath(Directory.GetCurrentDirectory())
//                .AddJsonFile("appsettings.json", optional: true)
//                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
//                .AddEnvironmentVariables()
//                .Build();

//var asdf2 = builder.Configuration.GetConnectionString("AppConfigConnectionString");
//string appConfigConnectionString = builder.Configuration.GetConnectionString("AppConfigConnectionString");
//builder.Configuration.AddAzureAppConfiguration(appConfigConnectionString);

//builder.Services.AddDbContext<SilvermineNordicDbContext>();
//builder.Services.AddSingleton<IWeatherForecast, OpenWeatherApiForecastService>();
//builder.Services.AddHttpClient();
//var asdf = builder.Configuration.GetSection("SilvermineNordic");
//builder.Services.Configure<SilvermineNordicConfigurationService>(builder.Configuration.GetSection("SilvermineNordic"));


var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        string appConfigConnectionString = Environment.GetEnvironmentVariable("AppConfigConnectionString");
        builder.AddAzureAppConfiguration(appConfigConnectionString);
    })
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddDbContext<SilvermineNordicDbContext>();
        services.AddSingleton<IWeatherForecast, OpenWeatherApiForecastService>();
        services.AddSingleton<ISilvermineNordicConfiguration, SilvermineNordicConfigurationService>();
        services.AddAzureAppConfiguration();
        services.AddHttpClient();
    })
    .Build();

host.Run();
