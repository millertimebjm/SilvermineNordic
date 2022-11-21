using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SilvermineNordic.Repository;
using SilvermineNordic.Repository.Services;

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
                    StorageConnectionString = null,
                    StorageName = null,
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

app.MapGet("/sensorreading/last", async () =>
{
    return await sensorReadingService.GetLatestSensorReadingAsync();
}).WithName("GetLastSensorReading");

app.MapGet("/weatherforecast", async () =>
{
    var weatherForecast = await weatherForecastService.GetWeatherForecast();
    return weatherForecast;
}).WithName("GetWeatherForecast");

app.MapGet("/weatherforecast/nextzonechange", async () =>
{
    var weatherForecast = await weatherForecastService.GetWeatherForecast();
    var sensorThreshold = await sensorThresholdService.GetThresholds();
    var lastSensorReading = await sensorReadingService.GetLatestSensorReadingAsync();
    
    var currentZone = InTheZoneService.IsInZone(sensorThreshold, lastSensorReading.TemperatureInCelcius, lastSensorReading.Humidity);

    int nextWeatherForecastIndex = 0;
    while (currentZone == InTheZoneService.IsInZone(sensorThreshold, weatherForecast.List[nextWeatherForecastIndex].Main.Temp, weatherForecast.List[nextWeatherForecastIndex].Main.Humidity)
        || weatherForecast.List[nextWeatherForecastIndex] == weatherForecast.List.Last())
    {
        nextWeatherForecastIndex++;
    }
    return weatherForecast.List[nextWeatherForecastIndex].DateTimeUtc;
}).WithName("GetNextZoneChange");

app.MapGet("thresholds", async () =>
{
    var thresholds = await sensorThresholdService.GetThresholds();
    return thresholds;
});

app.Run();