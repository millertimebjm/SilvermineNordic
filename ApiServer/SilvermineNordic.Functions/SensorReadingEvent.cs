using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SilvermineNordic.Models;
using SilvermineNordic.Repository.Services;

namespace SilvermineNordic.Functions
{
    public class SensorReadingEvent
    {
        private readonly ILogger _logger;
        private readonly IRepositoryReading _sensorReadingService;

        public SensorReadingEvent(
            ILoggerFactory loggerFactory,
            IRepositoryReading sensorReadingService)
        {
            _logger = loggerFactory.CreateLogger<SensorReadingEvent>();
            _sensorReadingService = sensorReadingService;
        }

        // http://localhost:7113/api/SensorReadingEvent?temperatureInCelcius=25&humidity=25
        // http://localhost:7113/api/SensorReadingEvent?temperatureInCelcius=15&humidity=15
        // http://localhost:7113/api/SensorReadingEvent?temperatureInCelcius=-5&humidity=32
        [Function("SensorReadingEvent")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "{temperatureInCelcius:decimal}/{humidity:decimal}")] HttpRequestData req,
            decimal temperatureInCelcius,
            decimal humidity)
        {
            var sensorReadingDateTimeUtc = DateTime.UtcNow;
            var queryStringArray = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            _logger.LogInformation($"C# HTTP trigger function processed a {req.Method.ToUpper()} request.");

            // var temperatureInCelciusString = queryStringArray["temperatureInCelcius"].ToString();
            // var humidityString = queryStringArray["humidity"].ToString();

            // if (string.IsNullOrWhiteSpace(temperatureInCelciusString)
            //     && string.IsNullOrWhiteSpace(humidityString))
            // {
            //     string requestBody = String.Empty;
            //     using (StreamReader streamReader = new StreamReader(req.Body))
            //     {
            //         requestBody = await streamReader.ReadToEndAsync();
            //         _logger.LogInformation($"RequestBody: {requestBody}");
            //     }
            //     dynamic data = JsonConvert.DeserializeObject(requestBody);
            //     if (data != null)
            //     {
            //         temperatureInCelciusString = data.temperatureInCelcius;
            //     }
            //     if (data != null)
            //     {
            //         humidityString = data.humidity;
            //     }
            // }

            // _logger.LogInformation($"TemperatureInCelcius value entered as {temperatureInCelciusString}.");
            // _logger.LogInformation($"Humidity value entered as {humidityString}.");

            // if (!decimal.TryParse(temperatureInCelciusString, out var temperatureInCelcius)
            //     || !decimal.TryParse(humidityString, out var humidity))
            // {
            //     _logger.LogInformation($"Query parameters not formatted correctly.");
            //     return req.CreateResponse(HttpStatusCode.BadRequest); // new BadRequestObjectResult("Query parameters not formatted correctly.");
            // }

            var insertedSensorReading = await _sensorReadingService.AddReadingAsync(new Reading()
            {
                Type = ReadingTypeEnum.Sensor.ToString(),
                TemperatureInCelcius = temperatureInCelcius,
                Humidity = humidity,
                ReadingDateTimestampUtc = sensorReadingDateTimeUtc,
            });
            _logger.LogInformation($"Inserted Sensor Reading Id: {insertedSensorReading.Id} | DateTime: {insertedSensorReading.DateTimestampUtc}");

            return req.CreateResponse(HttpStatusCode.OK); // new OkObjectResult("Event processed.");
        }
    }
}
