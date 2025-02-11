using Microsoft.Identity.Client;
using SilvermineNordic.Api.Services;
using SilvermineNordic.Common;
using SilvermineNordic.Models;
using SilvermineNordic.Repository.Services;

namespace SilvermineNordic.Api.Services;
public class ApiMapperService : IApiMapper
{
    private readonly IRepositoryReading _repositoryReadingService;
    private readonly IWeatherForecast _weatherForecastService;
    private readonly IRepositoryThreshold _repositoryThresholdService;


    public ApiMapperService(
        IRepositoryReading repositoryReadingService,
        IWeatherForecast weatherForecastService,
        IRepositoryThreshold repositoryThresholdService)
    {
        _repositoryReadingService = repositoryReadingService;
        _weatherForecastService = weatherForecastService;
        _repositoryThresholdService = repositoryThresholdService;
    }

    public void SetMaps(WebApplication app)
    {
        app.MapGet("/", () =>
        {
            return Results.Ok();
        });

        app.MapGet("/reading/{readingType}/{count?}/{skip?}",
            async (
                string readingType,
                int? count,
                int? skip) =>
        {
            var countNonNull = count ?? 1;
            countNonNull = countNonNull > 100 ? 100 : countNonNull;
            countNonNull = countNonNull < 1 ? 1 : countNonNull;
            var skipNonNull = skip ?? 0;
            return await _repositoryReadingService.GetLastNReadingAsync(
                Enum.Parse<ReadingTypeEnum>(readingType, ignoreCase: true),
                countNonNull,
                skipNonNull);
        }).WithName("GetRecentReadings");

        app.MapGet("/weatherforecast", async () =>
        {
            return await _weatherForecastService.GetWeatherForecast();
        }).WithName("GetWeatherForecast");

        app.MapGet("/weatherforecast/nextzonechange", async () =>
        {
            var weatherForecastTask = await _weatherForecastService.GetWeatherForecast();
            var thresholdTask = await _repositoryThresholdService.GetThresholds();
            var lastSensorReadingTask = await _repositoryReadingService.GetLastNReadingAsync(ReadingTypeEnum.Sensor, 1);
            var lastWeatherReadingTask = await _repositoryReadingService.GetLastNReadingAsync(ReadingTypeEnum.Weather, 1);
            //await Task.WhenAll(weatherForecastTask, thresholdTask, lastSensorReadingTask, lastWeatherReadingTask);

            var lastSensorReading = lastSensorReadingTask.Single();
            var lastWeatherReading = lastWeatherReadingTask.Single();
            var nextZoneChangeDateTimeUtc = InTheZoneService.GetNextZoneChange(weatherForecastTask, thresholdTask, InTheZoneService.IsInZone(thresholdTask, lastSensorReading.TemperatureInCelcius, lastSensorReading.Humidity) || InTheZoneService.IsInZone(thresholdTask, lastWeatherReading.TemperatureInCelcius, lastWeatherReading.Humidity));
            return nextZoneChangeDateTimeUtc;
        }).WithName("GetNextZoneChange");

        app.MapGet("threshold", async () =>
        {
            var thresholds = await _repositoryThresholdService.GetThresholds();
            return thresholds;
        });
    }
}