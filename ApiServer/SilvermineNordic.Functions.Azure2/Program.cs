// using Microsoft.Extensions.Hosting;

// var host = new HostBuilder()
//     .ConfigureFunctionsWorkerDefaults()
//     .Build();

// host.Run();


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SilvermineNordic.Repository.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SilvermineNordic.Repository;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var appConfigConnectionString = "Endpoint=https://snowmakingappconfig.azconfig.io;Id=WPMo;Secret=Yd7MZDYtoLqY0Y3deYOOn4MhfORh9Q7sJptB9UK1dq4=";
//Console.WriteLine($"AppConfigConnectionString: {config["AppConfigConnectionString"]}");
var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        // string appConfigConnectionString = config["AppConfigConnectionString"]
        //     ?? throw new ArgumentNullException("AppConfigConnectionString");
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
    .UseDefaultServiceProvider(options => options.ValidateScopes = false)
    .Build();

await host.RunAsync();
