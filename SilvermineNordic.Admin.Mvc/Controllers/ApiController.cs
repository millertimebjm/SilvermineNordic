using SilvermineNordic.Repository.Services;
using Microsoft.AspNetCore.Mvc;
using SilvermineNordic.Models;
using System;

public class ApiController : Controller
{
    private readonly ILogger<ApiController> _logger;
    private readonly IRepositoryReading _repositoryReadingService;
    private readonly IWeatherForecast _weatherService;

    public ApiController(
        ILogger<ApiController> logger,
        IRepositoryReading repositoryReadingService,
        IWeatherForecast weatherService) 
    {
        _logger = logger;
        _repositoryReadingService = repositoryReadingService;
        _weatherService = weatherService;
    }

/*
curl -X POST -H "Content-Type: application/json" \
-d '{"temperatureInCelcius": 23, "humidity": 60}' \
http://localhost:5025/api/sensorreading
*/
    [Route("api/sensorreading")]
    [HttpPost]
    public async Task<JsonResult> SensorReadingPost(
        [FromBody] Reading readingInput) 
    {
        var reading = new Reading()
        {
            Type = "Sensor",
            TemperatureInCelcius = readingInput.TemperatureInCelcius,
            Humidity = readingInput.Humidity,
            DateTimeUtc = DateTime.UtcNow,
        };
        reading = await _repositoryReadingService.AddReadingAsync(reading);
        return Json(reading);
    }

/*
curl -X POST \
http://localhost:5025/api/weatherreading
*/
    [Route("api/weatherreading")]
    [HttpPost]
    public async Task<JsonResult> WensorReadingPost() 
    {
        var weatherModel = await _weatherService.GetCurrentWeather();
        var reading = new Reading()
        {
            Type = "Weather",
            DateTimeUtc = weatherModel.DateTimeUtc,
            TemperatureInCelcius = weatherModel.TemperatureInCelcius,
            Humidity = weatherModel.Humidity,
        };
        reading = await _repositoryReadingService.AddReadingAsync(reading);
        return Json(reading);
    }

    [Route("api/weatherforecast")]
    [HttpGet]
    public async Task<JsonResult> WeatherForecastGet() 
    {
        var weatherModel = await _weatherService.GetWeatherForecast();
        return Json(weatherModel);
    }
}