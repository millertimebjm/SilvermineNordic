using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SilvermineNordic.Repository;
using SilvermineNordic.Repository.Models;
using SilvermineNordic.Repository.Services;
using System;

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

builder.Services.AddSingleton<SilvermineNordic.Repository.IConfiguration>(_ =>
                new ConfigurationService()
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
    var weatherForecastTask = weatherForecastService.GetWeatherForecast();
    var sensorThresholdTask = sensorThresholdService.GetThresholds();
    var lastSensorReadingTask = sensorReadingService.GetLastNReadingAsync(SensorReadingTypeEnum.Sensor, 1);
    await Task.WhenAll(weatherForecastTask, sensorThresholdTask, lastSensorReadingTask);
    
    var currentZone = InTheZoneService.IsInZone(sensorThresholdTask.Result, lastSensorReadingTask.Result.Single().TemperatureInCelcius, lastSensorReadingTask.Result.Single().Humidity);
    List<WeatherModel> weatherForecastModels = weatherForecastTask.Result.ToList();
    int nextWeatherForecastIndex = 0;
    while (nextWeatherForecastIndex >= weatherForecastModels.Count() && currentZone == InTheZoneService.IsInZone(sensorThresholdTask.Result, weatherForecastModels[nextWeatherForecastIndex].TemperatureInCelcius, weatherForecastModels[nextWeatherForecastIndex].Humidity)
        || weatherForecastModels[nextWeatherForecastIndex] == weatherForecastModels.Last())
    {
        nextWeatherForecastIndex++;
    }
    if (nextWeatherForecastIndex >= weatherForecastModels.Count())
    {
        return (DateTime?)null;
    }
    return weatherForecastModels[nextWeatherForecastIndex].DateTimeUtc;
}).WithName("GetNextZoneChange");

app.MapGet("thresholds", async () =>
{
    var thresholds = await sensorThresholdService.GetThresholds();
    return thresholds;
});

app.Run();