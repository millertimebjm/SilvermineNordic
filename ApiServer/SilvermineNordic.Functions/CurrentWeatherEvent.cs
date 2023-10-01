using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SilvermineNordic.Repository.Services;
using SilvermineNordic.Repository;
using SilvermineNordic.Models;

namespace SilvermineNordic.Functions
{
    public class CurrentWeatherEvent
    {
        private readonly ILogger _logger;
        private readonly IWeatherForecast _weatherForecastService;
        private readonly IRepositoryReading _sensorReadingService;

        public CurrentWeatherEvent(
            ILoggerFactory loggerFactory,
            IWeatherForecast weatherForecastService,
            IRepositoryReading readingService)
        {
            _logger = loggerFactory.CreateLogger<CurrentWeatherEvent>();
            _weatherForecastService = weatherForecastService;
            _sensorReadingService = readingService;
        }

        //[Function("CurrentWeatherEvent")]
        public async Task RunAsync([TimerTrigger("0 */5 * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var currentWeather = await _weatherForecastService.GetCurrentWeather();
            _logger.LogInformation($"Current Weather is TemperatureInCelcius: {currentWeather.TemperatureInCelcius} | Humidity: {currentWeather.Humidity}");
            var reading = await _sensorReadingService.AddReadingAsync(new Reading()
            {
                Type = ReadingTypeEnum.Weather.ToString(),
                TemperatureInCelcius = currentWeather.TemperatureInCelcius,
                Humidity = currentWeather.Humidity,
                DateTimestampUtc = currentWeather.DateTimeUtc,
                ReadingDateTimestampUtc = currentWeather.DateTimeUtc,
            });
            _logger.LogInformation($"Current Weather inserted with Id {reading.Id}.");
        }
    }
}
