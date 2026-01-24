using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SilvermineNordic.Models;
using SilvermineNordic.Repository;
using SilvermineNordic.Repository.Services;

namespace SilvermineNordic.RefreshWeather;
public class Program
{
    public static async Task Main(string[] args)
    {
        var serviceProvider = BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var readingByZipService = scope.ServiceProvider.GetRequiredService<IReadingByZip>();
        var zipApiService = scope.ServiceProvider.GetRequiredService<IZipApi>();
        var weatherForecastService = scope.ServiceProvider.GetRequiredService<IWeatherForecast>();

        var timer = new PeriodicTimer(TimeSpan.FromMinutes(15));

        while (await timer.WaitForNextTickAsync())
        {
            await DoRefresh(readingByZipService, zipApiService, weatherForecastService);
        }   
    }

    private static async Task DoRefresh(
        IReadingByZip readingByZipService,
        IZipApi zipApiService,
        IWeatherForecast weatherForecastService)
    {
        var lookups = await readingByZipService.GetForRefresh();
        var zipModels = lookups.Select(l => new ZipModelRoot() { ZipCode = l });

        foreach (var zipModel in zipModels)
        {
            var root = await zipApiService.GetLatLong(zipModel);
            var currentWeather = await weatherForecastService.GetWeatherForecast();
            var readingByZip = new ReadingByZip() { WeatherDataSerialized = JsonSerializer.Serialize(currentWeather) };
            await readingByZipService.Upsert(zipModel.ZipCode, readingByZip.WeatherDataSerialized);
        }
    }

    private static ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        const string _applicationNameConfigurationService = "SilvermineNordic";
        const string _appConfigEnvironmentVariableName = "AppConfigConnectionString";

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
            
        var configuration = builder.Build();

        string appConfigConnectionString =
            // Windows config value
            configuration[_appConfigEnvironmentVariableName]
            // Linux config value
            ?? configuration[$"Values:{_appConfigEnvironmentVariableName}"]
            ?? throw new ArgumentNullException(_appConfigEnvironmentVariableName);

        builder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddAzureAppConfiguration(appConfigConnectionString);

        configuration = builder.Build();

        services.AddSingleton<IReadingByZip, EntityFrameworkReadingByZipService>();
        services.AddSingleton<IWeatherForecast, OpenWeatherApiForecastService>();
        services.AddSingleton<IZipApi, ZippopotamZipService>();
        services.AddOptions<SilvermineNordicConfigurationService>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(_applicationNameConfigurationService).Bind(settings);
            });
        services.AddSingleton<IConfiguration>(configuration);

        services.AddHttpClient();

        var connectionString = configuration
            .GetValue(typeof(string), $"{_applicationNameConfigurationService}:SqlConnectionString")?
            .ToString();
        Console.WriteLine($"Connection String: {connectionString}");
        if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
        services.AddDbContext<SilvermineNordicDbContext>(opts => 
            opts.UseNpgsql(connectionString, options => options.MigrationsAssembly("SilvermineNordic.Admin.Mvc"))
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information), ServiceLifetime.Transient);

        // builder.Host.UseSerilog();
        // Log.Logger = new LoggerConfiguration()
        //     .MinimumLevel.Debug()
        //     .WriteTo.Console()
        //     .WriteTo.GrafanaLoki("http://media.bltmiller.com:3100", 
        //         labels: new[]
        //         {
        //             new LokiLabel() {Key = "app", Value = "snowmaking"},
        //             new LokiLabel() {Key = "env", Value = "prod"},
        //         })
        //     .CreateLogger();

        return services.BuildServiceProvider();
    }
}