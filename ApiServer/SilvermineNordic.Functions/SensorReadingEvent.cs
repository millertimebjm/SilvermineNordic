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

namespace SilvermineNordic.Functions
{
    public class SensorReadingEvent
    {
        private readonly IRepositorySensorReading _sensorReadingService;
        private readonly IRepositoryThreshold _thresholdService;
        public SensorReadingEvent(IRepositorySensorReading sensorReadingService, IRepositoryThreshold thresholdService)
        {
            _sensorReadingService = sensorReadingService;
            _thresholdService = thresholdService;
        }

// local.settings.json
//{
//  "IsEncrypted": false,
//  "ConnectionStrings": {
//    "SnowMakingSqlConnectionString": ""
//  },
//  "Values": {  }
//}

    // http://localhost:7113/api/SensorReadingEvent?temperatureInCelcius=25&humidity=25
    // http://localhost:7113/api/SensorReadingEvent?temperatureInCelcius=15&humidity=15
    [FunctionName("SensorReadingEvent")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
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
                    var sensorData = await _sensorReadingService.GetLatestSensorReadingAsync();
                    var thresholdData = await _thresholdService.GetThreshold();

                    var isInZoneBefore = IsInZone(thresholdData, sensorData.TemperatureInCelcius, sensorData.Humidity);
                    var isInZoneAfter = IsInZone(thresholdData, temperatureInCelcius, humidity);
                    if (isInZoneBefore != isInZoneAfter)
                    {
                        log.LogInformation("Threshold for notification has been reached!");
                    }
                    else
                    {
                        log.LogInformation("Threshold for notification NOT reached!");
                    }

                    var insertedSensorReading = await _sensorReadingService.AddSensorReadingAsync(new SensorReading()
                    {
                        TemperatureInCelcius = temperatureInCelcius,
                        Humidity = humidity,
                    });
                    log.LogInformation($"Inserted Sensor Reading Id: {insertedSensorReading.Id} | DateTime: {insertedSensorReading.DateTimestampUtc}");

                    return new OkObjectResult("Event processed.");
                }
                catch (Exception ex)
                {
                    return new BadRequestObjectResult(ex.Message);
                }
            }

            return new BadRequestObjectResult("Query parameters not formatted correctly.");
        }

        private static bool IsInZone(Threshold thresholdData, decimal lastTemperatureInCelcius, decimal lastHumidity)
        {
            if (lastTemperatureInCelcius > thresholdData.TemperatureInCelciusLowThreshold
                && lastTemperatureInCelcius < thresholdData.TemperatureInCelciusHighThreshold
                && lastHumidity > thresholdData.HumidityLowThreshold
                && lastHumidity < thresholdData.HumidityHighThreshold)
            {
                return true;
            }
            return false;
        }
    }
}
