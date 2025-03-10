using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SilvermineNordic.Admin.Mvc.Models;
using SilvermineNordic.Repository.Services;
using System.Threading.Tasks;
using SilvermineNordic.Models;
using SilvermineNordic.Common;

namespace SilvermineNordic.Admin.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRepositoryReading _repositoryReadingService;
    private readonly IWeatherForecast _weatherForecastService;
    private readonly IRepositoryThreshold _repositoryThresholdService;

    public HomeController(
        ILogger<HomeController> logger,
        IRepositoryReading repositoryReadingService,
        IWeatherForecast weatherForecastService,
        IRepositoryThreshold repositoryThresholdService)
    {
        _logger = logger;
        _repositoryReadingService = repositoryReadingService;
        _weatherForecastService = weatherForecastService;
        _repositoryThresholdService = repositoryThresholdService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new IndexModel();
        model.SensorReadingsTask = (Task<IEnumerable<Reading>>)_repositoryReadingService.GetLastNReadingAsync(ReadingTypeEnum.Sensor, 5);
        await model.SensorReadingsTask;
        model.WeatherReadingsTask = (Task<IEnumerable<Reading>>)_repositoryReadingService.GetLastNReadingAsync(ReadingTypeEnum.Weather, 5);
        await model.WeatherReadingsTask;
        model.ThresholdsTask = (Task<IEnumerable<Threshold>>)_repositoryThresholdService.GetThresholds();
        await model.ThresholdsTask;
        model.WeatherForecastTask = GetWeatherForecastWithZone(model.ThresholdsTask);
        await model.WeatherForecastTask;
        model.NextZoneChangeTask = GetNextZoneChange(model);
        await model.NextZoneChangeTask;
        return View(model);
    }

    private async Task<IEnumerable<WeatherModelWithZone>> GetWeatherForecastWithZone(
        Task<IEnumerable<Threshold>> thresholdsTask)
    {
        var weatherForecastModel = await _weatherForecastService.GetWeatherForecast();
        var weatherForecastWithZoneModel = new List<WeatherModelWithZone>();
        foreach (var forecast in weatherForecastModel)
        {
            weatherForecastWithZoneModel.Add(new WeatherModelWithZone()
            {
                DateTimeUtc = forecast.DateTimeUtc,
                TemperatureInCelcius = forecast.TemperatureInCelcius,
                FeelsLikeInCelcius = forecast.FeelsLikeInCelcius,
                Humidity = forecast.Humidity,
                SnowfallInCm = forecast.SnowfallInCm,
                RainfallInCm = forecast.RainfallInCm,
                InTheZone = InTheZoneService.IsInZone(
                    await thresholdsTask, 
                    forecast.TemperatureInCelcius, 
                    forecast.Humidity
                ),
                CloudPercentage = forecast.CloudPercentage,
            });
            Console.WriteLine(forecast.CloudPercentage);
        }
        return weatherForecastWithZoneModel;
    }

    private async Task<DateTime?> GetNextZoneChange(IndexModel model)
    {
        var sensorReadings = await model.SensorReadingsTask;
        var weatherReadings = await model.WeatherReadingsTask;
        var thresholds = await model.ThresholdsTask;

        var lastReading = sensorReadings.FirstOrDefault();
        if ((lastReading?.DateTimeUtc ?? DateTime.MinValue) <
        ((weatherReadings).FirstOrDefault()?.DateTimeUtc ??
        DateTime.MinValue))
            lastReading = weatherReadings.First();
        var inTheZone = false;
        if (lastReading != null)
        {
            inTheZone = SilvermineNordic.Common.InTheZoneService.IsInZone(thresholds, lastReading.TemperatureInCelcius,
                lastReading.Humidity);
            return InTheZoneService.GetNextZoneChange(await model.WeatherForecastTask, await model.ThresholdsTask, inTheZone);
        }
        return (DateTime?)null;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
