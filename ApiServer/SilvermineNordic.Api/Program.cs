using SilvermineNordic.Repository;
using SilvermineNordic.Models;
using SilvermineNordic.Repository.Services;
using SilvermineNordic.Common;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

const string _applicationNameConfigurationService = "SilvermineNordic";
const string _appConfigEnvironmentVariableName = "AppConfigConnectionString";

builder
    .Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

string appConfigConnectionString =
            // Windows config value
            builder.Configuration[_appConfigEnvironmentVariableName]
            // Linux config value
            ?? builder.Configuration[$"Values:{_appConfigEnvironmentVariableName}"]
            ?? throw new ArgumentNullException(_appConfigEnvironmentVariableName);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddAzureAppConfiguration(appConfigConnectionString)
    .Build();

builder.Services.AddOptions<SilvermineNordicConfigurationService>()
    .Configure<IConfiguration>((settings, configuration) =>
    {
        configuration.GetSection(_applicationNameConfigurationService).Bind(settings);
    });
builder.Services.AddDbContext<SilvermineNordicDbContext>(ServiceLifetime.Scoped);
builder.Services.AddScoped<IRepositoryReading, EntityFrameworkReadingService>();
builder.Services.AddScoped<IRepositoryThreshold, EntityFrameworkThresholdService>();
builder.Services.AddScoped<IWeatherForecast, OpenWeatherApiForecastService>();
builder.Services.AddScoped<IRepositoryUser, EntityFrameworkUserService>();
builder.Services.AddScoped<IRepositoryUserOtp, EntityFrameworkUserOtpService>();
builder.Services.AddCors(o => o.AddPolicy("NUXT", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});

var app = builder.Build();

app.UseCors("NUXT");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Console.WriteLine($"OpenWeatherForecastApiKey: |{configService.GetOpenWeatherApiKey()}|");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/reading/{readingType}/{count?}/{skip?}",
    async (
        string readingType,
        int? count,
        int? skip) =>
{
    using (var scope = app.Services.CreateScope())
    {
        var sensorReadingService = scope.ServiceProvider.GetRequiredService<IRepositoryReading>();
        var countNonNull = count ?? 1;
        countNonNull = countNonNull > 100 ? 100 : countNonNull;
        countNonNull = countNonNull < 1 ? 1 : countNonNull;
        var skipNonNull = skip ?? 0;
        return await sensorReadingService.GetLastNReadingAsync(
            Enum.Parse<ReadingTypeEnum>(readingType, ignoreCase: true),
            countNonNull,
            skipNonNull);
    }
}).WithName("GetRecentReadings");

app.MapGet("/weatherforecast", async () =>
{
    using (var scope = app.Services.CreateScope())
    {
        var weatherForecastService = scope.ServiceProvider.GetRequiredService<IWeatherForecast>();
        return await weatherForecastService.GetWeatherForecast();
    }
}).WithName("GetWeatherForecast");

app.MapGet("/weatherforecast/nextzonechange", async () =>
{
    using (var scope = app.Services.CreateScope())
    {
        var weatherForecastService = scope.ServiceProvider.GetRequiredService<IWeatherForecast>();
        var thresholdService = scope.ServiceProvider.GetRequiredService<IRepositoryThreshold>();
        var readingService = scope.ServiceProvider.GetRequiredService<IRepositoryReading>();

        var weatherForecastTask = await weatherForecastService.GetWeatherForecast();
        var thresholdTask = await thresholdService.GetThresholds();
        var lastSensorReadingTask = await readingService.GetLastNReadingAsync(ReadingTypeEnum.Sensor, 1);
        var lastWeatherReadingTask = await readingService.GetLastNReadingAsync(ReadingTypeEnum.Weather, 1);
        //await Task.WhenAll(weatherForecastTask, thresholdTask, lastSensorReadingTask, lastWeatherReadingTask);

        var lastSensorReading = lastSensorReadingTask.Single();
        var lastWeatherReading = lastWeatherReadingTask.Single();
        var nextZoneChangeDateTimeUtc = InTheZoneService.GetNextZoneChange(weatherForecastTask, thresholdTask, InTheZoneService.IsInZone(thresholdTask, lastSensorReading.TemperatureInCelcius, lastSensorReading.Humidity) || InTheZoneService.IsInZone(thresholdTask, lastWeatherReading.TemperatureInCelcius, lastWeatherReading.Humidity));
        return nextZoneChangeDateTimeUtc;
    }
}).WithName("GetNextZoneChange");

app.MapGet("threshold", async () =>
{
    using (var scope = app.Services.CreateScope())
    {
        var thresholdService = scope.ServiceProvider.GetRequiredService<IRepositoryThreshold>();
        var thresholds = await thresholdService.GetThresholds();
        return thresholds;
    }
});

app.MapPost("loginattempt", async ([Microsoft.AspNetCore.Mvc.FromBody] string login) =>
{
    using (var scope = app.Services.CreateScope())
    {
        var userService = scope.ServiceProvider.GetRequiredService<EntityFrameworkUserService>();
        var emailService = scope.ServiceProvider.GetRequiredService<AzureEmailService>();
        var userOtpService = scope.ServiceProvider.GetRequiredService<IRepositoryUserOtp>();
        try
        {
            var user = await userService.GetUserAsync(login);
            if (user != null)
            {
                var userOtpResult = await userOtpService.AddUserOtpAsync(user.Id);
                var emailResult = await emailService.SendEmailAsync(user.Email, "Snow Making Login Request", @$"Dear {(!string.IsNullOrWhiteSpace(user.Name) ? user.Name : user.Email)},{Environment.NewLine}<br/>https://snowmaking.silverminenordic.com/loginotp/{userOtpResult.Otp}");
                return true;
            }
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
});

app.MapGet("loginotp", async (string otp) =>
{
    using (var scope = app.Services.CreateScope())
    {
        var userOtpService = scope.ServiceProvider.GetRequiredService<IRepositoryUserOtp>();
        if (Guid.TryParse(otp, out Guid otpGuid))
        {
            return await userOtpService.GetUserOtpAsync(otpGuid);
        }
        return (UserOtp)null;
    }
});

app.MapGet("loginauth", async (string authKey) =>
{
    using (var scope = app.Services.CreateScope())
    {
        var userOtpService = scope.ServiceProvider.GetRequiredService<IRepositoryUserOtp>();
        if (Guid.TryParse(authKey, out Guid authKeyGuid))
        {
            return await userOtpService.GetUserOtpByAuthKeyAsync(authKeyGuid);
        }
        return (User)null;
    }
});

app.Run();

