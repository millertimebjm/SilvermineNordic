using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SilvermineNordic.Repository.Services;
using Microsoft.Extensions.DependencyInjection;
using SilvermineNordic.Repository;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

const string _applicationNameConfigurationService = "SilvermineNordic";
const string _appConfigEnvironmentVariableName = "AppConfigConnectionString";

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        string appConfigConnectionString =
            // Windows config value
            config[_appConfigEnvironmentVariableName]
            // Linux config value
            ?? config[$"Values:{_appConfigEnvironmentVariableName}"]
            ?? throw new ArgumentNullException(_appConfigEnvironmentVariableName);
        builder.AddAzureAppConfiguration(options =>
        {
            options
                .Connect(appConfigConnectionString)
                .Select($"{_applicationNameConfigurationService}:*", LabelFilter.Null);
        });
    })
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddDbContext<SilvermineNordicDbContext>();
        services.AddSingleton<IWeatherForecast, OpenWeatherApiForecastService>();
        services.AddScoped<IRepositoryReading, EntityFrameworkReadingService>();
        services.AddScoped<IRepositoryThreshold, EntityFrameworkThresholdService>();
        services.AddOptions<SilvermineNordicConfigurationService>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(_applicationNameConfigurationService).Bind(settings);
            });
        services.AddHttpClient();
    })
    .UseDefaultServiceProvider(options => options.ValidateScopes = false)
    .Build();

await host.RunAsync();
