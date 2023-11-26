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
        model.WeatherReadingsTask = (Task<IEnumerable<Reading>>)_repositoryReadingService.GetLastNReadingAsync(ReadingTypeEnum.Weather, 5);
        model.ThresholdsTask = (Task<IEnumerable<Threshold>>)_repositoryThresholdService.GetThresholds();
        model.WeatherForecastTask = (Task<IEnumerable<WeatherModel>>)_weatherForecastService.GetWeatherForecast();
        
        model.NextZoneChangeTask = Task.Run(async () => {
            var sensorReadings = await model.SensorReadingsTask;
            var weatherReadings = await model.WeatherReadingsTask;
            var thresholds = await model.ThresholdsTask;

            var lastReading = sensorReadings.FirstOrDefault();
            if ((lastReading?.ReadingDateTimestampUtc ?? DateTime.MinValue) <
            ((weatherReadings).FirstOrDefault()?.ReadingDateTimestampUtc ??
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
        });
        return View(model);
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
