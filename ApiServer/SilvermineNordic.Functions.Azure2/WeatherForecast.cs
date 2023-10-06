using System.ComponentModel;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SilvermineNordic.Repository;
using SilvermineNordic.Repository.Services;

namespace SilvermineNordic.Functions.Azure
{
    public class WeatherForecast
    {
        private readonly ILogger _logger;
        private readonly SilvermineNordicConfigurationService _settings;
        private readonly IWeatherForecast _weatherForecastService;

        public WeatherForecast(ILoggerFactory loggerFactory,
            IOptionsSnapshot<SilvermineNordicConfigurationService> options,
            IWeatherForecast weatherForecastService)
        {
            _logger = loggerFactory.CreateLogger<WeatherForecast>();
            _settings = options.Value;
            _weatherForecastService = weatherForecastService;
        }

        [Function("WeatherForecast")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");
            Console.WriteLine($"OpenWeatherApiKey: {_settings.OpenWeatherApiKey}");
            Thread.Sleep(5000);
            var weather = await _weatherForecastService.GetCurrentWeather();


            return response;
        }
    }
}
