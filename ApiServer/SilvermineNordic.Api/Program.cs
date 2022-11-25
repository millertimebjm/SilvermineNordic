using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SilvermineNordic.Repository;
using SilvermineNordic.Models;
using SilvermineNordic.Repository.Services;
using System;
using SilvermineNordic.Common;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var executionContextOptions = builder.Services.BuildServiceProvider()
                .GetRequiredService<IOptions<ExecutionContextOptions>>().Value;

var config = new ConfigurationBuilder()
                //.SetBasePath(executionContextOptions.AppDirectory)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

string snowMakingSqlConnectionString = config.GetConnectionString("SnowMakingSqlConnectionString");
string openWeatherApiKey = config.GetConnectionString("OpenWeatherApiForecastApiKey");

builder.Services.AddSingleton<SilvermineNordic.Repository.ISilvermineNordicConfiguration>(_ =>
                new SilvermineNordicConfigurationService()
                {
                    SqlConnectionString = snowMakingSqlConnectionString,
                    OpenWeatherApiKey = openWeatherApiKey
                });

builder.Services.AddDbContext<SilvermineNordicDbContext>();

builder.Services.AddScoped<IRepositorySensorReading, EntityFrameworkSensorReadingService>();
builder.Services.AddScoped<IRepositoryThreshold, EntityFrameworkThresholdService>();
builder.Services.AddScoped<IWeatherForecast, OpenWeatherApiForecastService>();

var sensorReadingService = builder.Services.BuildServiceProvider().GetService<IRepositorySensorReading>();
var sensorThresholdService = builder.Services.BuildServiceProvider().GetService<IRepositoryThreshold>();
var weatherForecastService = builder.Services.BuildServiceProvider().GetService<IWeatherForecast>();

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/sensorreading/", async (int count) =>
{
    count = count > 100 ? 100 : count;
    count = count < 1 ? 1 : count;
    return await sensorReadingService.GetLastNReadingAsync(SensorReadingTypeEnum.Sensor, count);
}).WithName("GetLastSensorReading");

app.MapGet("/weatherreading/", async (int count) =>
{
    count = count > 100 ? 100 : count;
    count = count < 1 ? 1 : count;
    return await sensorReadingService.GetLastNReadingAsync(SensorReadingTypeEnum.Weather, count);
}).WithName("GetLastWeatherReading");

app.MapGet("/weatherforecast", async () =>
{
    return await weatherForecastService.GetWeatherForecast();
}).WithName("GetWeatherForecast");

app.MapGet("/weatherforecast/nextzonechange", async () =>
{
    var weatherForecastTask = await weatherForecastService.GetWeatherForecast();
    var thresholdTask = await sensorThresholdService.GetThresholds();
    var lastSensorReadingTask = await sensorReadingService.GetLastNReadingAsync(SensorReadingTypeEnum.Sensor, 1);
    var lastWeatherReadingTask = await sensorReadingService.GetLastNReadingAsync(SensorReadingTypeEnum.Weather, 1);
    //await Task.WhenAll(weatherForecastTask, thresholdTask, lastSensorReadingTask, lastWeatherReadingTask);

    var lastSensorReading = lastSensorReadingTask.Single();
    var lastWeatherReading = lastWeatherReadingTask.Single();
    var nextZoneChangeDateTimeUtc = InTheZoneService.GetNextZoneChange(weatherForecastTask, thresholdTask, InTheZoneService.IsInZone(thresholdTask, lastSensorReading.TemperatureInCelcius, lastSensorReading.Humidity) || InTheZoneService.IsInZone(thresholdTask, lastWeatherReading.TemperatureInCelcius, lastWeatherReading.Humidity));
    return nextZoneChangeDateTimeUtc;
}).WithName("GetNextZoneChange");

app.MapGet("thresholds", async () =>
{
    var thresholds = await sensorThresholdService.GetThresholds();
    return thresholds;
});

app.Run();
