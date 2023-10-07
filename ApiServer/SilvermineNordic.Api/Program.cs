using SilvermineNordic.Repository;
using SilvermineNordic.Models;
using SilvermineNordic.Repository.Services;
using SilvermineNordic.Common;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

string snowMakingSqlConnectionString = config.GetConnectionString("SnowMakingSqlConnectionString");
string openWeatherApiKey = config["OpenWeatherApiForecastApiKey"];
string emailServiceConnectionString = config.GetConnectionString("EmailServiceConnectionString");

var configService = new SilvermineNordicConfigurationService()
{
    SqlConnectionString = snowMakingSqlConnectionString,
    OpenWeatherApiKey = openWeatherApiKey,
    EmailServiceConnectionString = emailServiceConnectionString,
    InMemoryDatabaseName = "SilvermineNordicDatabase",
};
builder.Services.AddSingleton<ISilvermineNordicConfiguration>(_ => configService);
builder.Services.AddDbContext<SilvermineNordicDbContext>(ServiceLifetime.Scoped);
builder.Services.AddScoped<IRepositoryReading, EntityFrameworkReadingService>();
builder.Services.AddScoped<IRepositoryThreshold, EntityFrameworkThresholdService>();
builder.Services.AddScoped<IWeatherForecast, OpenWeatherApiForecastService>();
builder.Services.AddScoped<IRepositoryUser, EntityFrameworkUserService>();
builder.Services.AddScoped<IRepositoryUserOtp, EntityFrameworkUserOtpService>();
builder.Services.AddScoped<IEmailService, AzureEmailService>();
builder.Services.AddScoped<IRepositoryUserOtp, EntityFrameworkUserOtpService>();

builder.Services.AddCors(o => o.AddPolicy("NUXT", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

var app = builder.Build();

app.UseCors("NUXT");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Console.WriteLine($"OpenWeatherForecastApiKey: |{configService.GetOpenWeatherApiKey()}|");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/sensorreading/{count?}", async (int? count) =>
{
    using (var scope = app.Services.CreateScope())
    {
        var sensorReadingService = scope.ServiceProvider.GetRequiredService<IRepositoryReading>();
        var countNonNull = count ?? 1;
        countNonNull = countNonNull > 100 ? 100 : countNonNull;
        countNonNull = countNonNull < 1 ? 1 : countNonNull;
        return await sensorReadingService.GetLastNReadingAsync(ReadingTypeEnum.Sensor, countNonNull);
    }
}).WithName("GetLastSensorReading");

app.MapGet("/weatherreading/{count?}", async (int? count) =>
{
    using (var scope = app.Services.CreateScope())
    {
        var sensorReadingService = scope.ServiceProvider.GetRequiredService<IRepositoryReading>();
        var countNonNull = count ?? 1;
        countNonNull = countNonNull > 100 ? 100 : countNonNull;
        countNonNull = countNonNull < 1 ? 1 : countNonNull;
        return await sensorReadingService.GetLastNReadingAsync(ReadingTypeEnum.Weather, countNonNull);
    }
}).WithName("GetLastWeatherReading");

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

app.MapGet("thresholds", async () =>
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

