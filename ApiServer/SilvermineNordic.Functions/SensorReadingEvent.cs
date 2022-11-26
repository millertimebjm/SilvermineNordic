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
using SilvermineNordic.Models;

namespace SilvermineNordic.Functions
{
    public class SensorReadingEvent
    {
        private readonly IRepositorySensorReading _sensorReadingService;
        public SensorReadingEvent(IRepositorySensorReading sensorReadingService)
        {
            _sensorReadingService = sensorReadingService;
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
    }
}
