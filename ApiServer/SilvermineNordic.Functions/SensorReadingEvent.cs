using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SilvermineNordic.Repository.Services;
using SilvermineNordic.Repository.Models;
using System.Linq;

namespace SilvermineNordic.Functions
{
    public class SensorReadingEvent
    {
        private readonly IRepositorySensorReading _sensorReadingService;
        private readonly IRepositoryThreshold _thresholdService;
        private readonly ISms _smsService;
        private readonly IWeatherForecast _weatherForecastService;

        public SensorReadingEvent(
            IRepositorySensorReading sensorReadingService,
            IRepositoryThreshold thresholdService,
            ISms smsService,
            IWeatherForecast weatherForecastService)
        {
            _sensorReadingService = sensorReadingService;
            _thresholdService = thresholdService;
            _smsService = smsService;
            _weatherForecastService = weatherForecastService;
        }

        // http://localhost:7113/api/SensorReadingEvent?temperatureInCelcius=25&humidity=25
        // http://localhost:7113/api/SensorReadingEvent?temperatureInCelcius=15&humidity=15
        // http://localhost:7113/api/SensorReadingEvent?temperatureInCelcius=-5&humidity=32
        [FunctionName("SensorReadingEvent")]
        public async Task<IActionResult> Run(
               [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
               ILogger log)
        {
            var sensorReadingDateTimeUtc = DateTime.UtcNow;
            log.LogInformation($"C# HTTP trigger function processed a {req.Method.ToUpper()} request.");

            var temperatureInCelciusString = req.Query["temperatureInCelcius"].ToString();
            var humidityString = req.Query["humidity"].ToString();

            if (string.IsNullOrWhiteSpace(temperatureInCelciusString)
                && string.IsNullOrWhiteSpace(humidityString))
            {
                string requestBody = String.Empty;
                using (StreamReader streamReader = new StreamReader(req.Body))
                {
                    requestBody = await streamReader.ReadToEndAsync();
                    log.LogInformation($"RequestBody: {requestBody}");
                }
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                if (data != null)
                {
                    temperatureInCelciusString = data.temperatureInCelcius;
                }
                if (data != null)
                {
                    humidityString = data.humidity;
                }
            }

            log.LogInformation($"TemperatureInCelcius value entered as {temperatureInCelciusString}.");
            log.LogInformation($"Humidity value entered as {humidityString}.");

            if (decimal.TryParse(temperatureInCelciusString, out var temperatureInCelcius)
                && decimal.TryParse(humidityString, out var humidity))
            {
                try
                {
                    var sensorData = (await _sensorReadingService.GetLastNReadingAsync(SensorReadingTypeEnum.Sensor, 1)).Single();
                    var thresholdData = await _thresholdService.GetThresholds();
                    var currentWeather = await _weatherForecastService.GetCurrentWeather();
                    log.LogInformation($"Current Weather is TemperatureInCelcius: {currentWeather.TemperatureInCelcius} | Humidity: {currentWeather.Humidity}");

                    var isInZoneSensorBefore = InTheZoneService.IsInZone(thresholdData, sensorData.TemperatureInCelcius, sensorData.Humidity);
                    var isInZoneSensorAfter = InTheZoneService.IsInZone(thresholdData, temperatureInCelcius, humidity);
                    var isInZoneWeather = InTheZoneService.IsInZone(thresholdData, currentWeather.TemperatureInCelcius, currentWeather.Humidity);
                    if (isInZoneSensorBefore != isInZoneSensorAfter || isInZoneWeather != isInZoneSensorBefore)
                    {
                        var message = "";
                        if (!isInZoneSensorBefore && isInZoneSensorAfter && isInZoneWeather)
                        {
                            message = "Sensor and Weather say it is now snow making time!";
                        }
                        else if (!isInZoneSensorBefore && isInZoneSensorAfter && !isInZoneWeather)
                        {
                            message = "Sensor says it's time to make snow, Weather says not yet.";
                        }
                        else if (!isInZoneSensorBefore && !isInZoneSensorAfter && isInZoneWeather)
                        {
                            message = "Weather says it's time to make snow, Sensor says not yet.";
                        }
                        else if (isInZoneSensorBefore && !isInZoneSensorAfter && !isInZoneWeather)
                        {
                            message = "Weather says snow making time is done, Sensor says not yet.";
                        }
                        else if (isInZoneSensorBefore && !isInZoneSensorAfter && isInZoneWeather)
                        {
                            message = "Sensor says snow making time is done, Weather says not yet.";
                        }
                        else if (isInZoneSensorBefore && isInZoneSensorAfter && !isInZoneWeather)
                        {
                            message = "Weather says snow making time is done, Sensor says not yet.";
                        }

                        var nextZoneChange = await _weatherForecastService.GetNextZoneChange(thresholdData, isInZoneSensorAfter || isInZoneWeather);
                        message += $" Next change forecasted for {nextZoneChange.Value.ToShortDateString()} {nextZoneChange.Value.ToShortTimeString()} UTC";
                        log.LogInformation(message);
                        await _smsService.SendSms("+17155239481", message);
                        await _smsService.SendSms("+17155792999", message);
                    }
                    else
                    {
                        log.LogInformation("Threshold for notification NOT reached.");
                    }

                    var insertedSensorReading = await _sensorReadingService.AddSensorReadingAsync(new SensorReading()
                    {
                        Type = SensorReadingTypeEnum.Sensor.ToString(),
                        TemperatureInCelcius = temperatureInCelcius,
                        Humidity = humidity,
                        ReadingDateTimestampUtc = sensorReadingDateTimeUtc,
                    });
                    log.LogInformation($"Inserted Sensor Reading Id: {insertedSensorReading.Id} | DateTime: {insertedSensorReading.DateTimestampUtc}");

                    return new OkObjectResult("Event processed.");
                }
                catch (Exception ex)
                {
                    log.LogInformation($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                    return new BadRequestObjectResult(ex.Message);
                }
            }

            log.LogInformation($"Query parameters not formatted correctly.");
            return new BadRequestObjectResult("Query parameters not formatted correctly.");
        }

        //private static DateTime ConvertUtcToAmericaChicago(DateTime input)
        //{
        //    string timezoneName = "America/Chicago";
        //    var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timezoneName);
        //    return input.Add(localTimeZone.BaseUtcOffset);
        //}
    }
}
