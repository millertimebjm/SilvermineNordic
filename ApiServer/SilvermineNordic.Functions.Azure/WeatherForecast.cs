using System.ComponentModel;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SilvermineNordic.Repository.Services;

namespace SilvermineNordic.Functions.Azure
{
    public class WeatherForecast
    {
        private readonly ILogger _logger;
        private readonly IWeatherForecast _weatherForecastService;
        public WeatherForecast(
            ILoggerFactory loggerFactory,
            IWeatherForecast weatherForecastService)
        {
            _logger = loggerFactory.CreateLogger<WeatherForecast>();
            _weatherForecastService = weatherForecastService;
        }

        [Function("WeatherForecast")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function WeatherForecast processed a request.");
            var weather = await _weatherForecastService.GetCurrentWeather();
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(weather);
            _logger.LogInformation("C# HTTP trigger function complete.");
            return response;
        }
    }
}
