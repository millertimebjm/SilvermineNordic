using System.ComponentModel;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SilvermineNordic.Models;
using SilvermineNordic.Repository.Services;

namespace SilvermineNordic.Functions.Azure
{
    public class WeatherReadingEvent
    {
        private readonly ILogger _logger;
        private readonly IWeatherForecast _weatherForecastService;
        private readonly IRepositoryReading _readingService;
        public WeatherReadingEvent(
            ILoggerFactory loggerFactory,
            IWeatherForecast weatherForecastService,
            IRepositoryReading readingService)
        {
            _logger = loggerFactory.CreateLogger<WeatherForecast>();
            _weatherForecastService = weatherForecastService;
            _readingService = readingService;
        }

        [Function("CreateWeatherReading")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function WeatherForecast processed a request.");
            var weather = await _weatherForecastService.GetCurrentWeather();
            var reading = new Reading(weather);
            reading = await _readingService.AddReadingAsync(reading);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(reading);
            _logger.LogInformation("C# HTTP trigger function complete.");
            return response;
        }
    }
}
