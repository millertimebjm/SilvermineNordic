using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SilvermineNordic.Models;
using SilvermineNordic.Repository.Services;

namespace SilvermineNordic.Functions.Azure
{
    public class SensorReadingEvent
    {
        private readonly ILogger _logger;
        private readonly IRepositoryReading _readingService;

        public SensorReadingEvent(
            ILoggerFactory loggerFactory,
            IRepositoryReading readingService)
        {
            _logger = loggerFactory.CreateLogger<SensorReadingEvent>();
            _readingService = readingService;
        }

        // http://localhost:7071/api/SensorReadingEvent?temperatureInCelcius=25&humidity=25
        // http://localhost:7071/api/SensorReadingEvent?temperatureInCelcius=15&humidity=15
        // http://localhost:7071/api/SensorReadingEvent?temperatureInCelcius=-5&humidity=32
        [Function("CreateSensorReadingEvent")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
            decimal temperatureInCelcius,
            decimal humidity)
        {
            var insertedSensorReading = await _readingService.AddReadingAsync(new Reading()
            {
                Type = ReadingTypeEnum.Sensor.ToString(),
                TemperatureInCelcius = temperatureInCelcius,
                Humidity = humidity,
                ReadingDateTimestampUtc = DateTime.UtcNow,
            });
            _logger.LogInformation($"Inserted Sensor Reading Id: {insertedSensorReading.Id}");

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(insertedSensorReading);
            return response;
        }
    }
}
