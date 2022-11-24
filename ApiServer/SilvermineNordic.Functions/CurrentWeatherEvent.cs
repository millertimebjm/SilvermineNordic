using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SilvermineNordic.Repository.Services;
using SilvermineNordic.Models;
using Microsoft.AspNetCore.Mvc;

namespace SilvermineNordic.Functions
{
    public class CurrentWeatherEvent
    {
        private readonly IWeatherForecast _weatherForecastService;
        private readonly IRepositorySensorReading _sensorReadingService;
        public CurrentWeatherEvent(IWeatherForecast weatherForecastService, IRepositorySensorReading sensorReadingService)
        {
            _weatherForecastService = weatherForecastService;
            _sensorReadingService = sensorReadingService;
        }

        [FunctionName("CurrentWeatherEvent")]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var currentWeather = await _weatherForecastService.GetCurrentWeather();
            log.LogInformation($"Current Weather is TemperatureInCelcius: {currentWeather.TemperatureInCelcius} | Humidity: {currentWeather.Humidity}");
            var reading = await _sensorReadingService.AddSensorReadingAsync(new SensorReading()
            {
                Type = SensorReadingTypeEnum.Weather.ToString(),
                TemperatureInCelcius = currentWeather.TemperatureInCelcius,
                Humidity = currentWeather.Humidity,
                DateTimestampUtc = currentWeather.DateTimeUtc,
                ReadingDateTimestampUtc = currentWeather.DateTimeUtc,
            });
            log.LogInformation($"Current Weather inserted with Id {reading.Id}.");
            //return new OkObjectResult($"Event processed with Id {reading.Id}.");
        }
    }
}
