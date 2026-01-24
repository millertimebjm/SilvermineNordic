using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SilvermineNordic.Admin.Mvc.Models;
using SilvermineNordic.Repository.Services;
using SilvermineNordic.Models;
using SilvermineNordic.Common;
using SilvermineNordic.Repository;
using System.Text.Json;

namespace SilvermineNordic.Admin.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRepositoryReading _repositoryReadingService;
    private readonly IWeatherForecast _weatherForecastService;
    private readonly IRepositoryThreshold _repositoryThresholdService;
    private readonly IZipApi _zipApiService;
    private readonly IReadingByZip _readingByZipService;

    public HomeController(
        ILogger<HomeController> logger,
        IRepositoryReading repositoryReadingService,
        IWeatherForecast weatherForecastService,
        IRepositoryThreshold repositoryThresholdService,
        IZipApi zipApiService,
        IReadingByZip readingByZipService)
    {
        _logger = logger;
        _repositoryReadingService = repositoryReadingService;
        _weatherForecastService = weatherForecastService;
        _repositoryThresholdService = repositoryThresholdService;
        _zipApiService = zipApiService;
        _readingByZipService = readingByZipService;
    }

    public async Task<IActionResult> Index(IndexPreferenceModel preferenceModel)
    {
        //var sensorReadingsTask = (Task<IEnumerable<Reading>>)_repositoryReadingService.GetLastNReadingAsync(ReadingTypeEnum.Sensor, 5);
        var zipModel = Task.FromResult(new ZipModelRoot());
        if (string.IsNullOrWhiteSpace(preferenceModel.zipCode)) preferenceModel = preferenceModel with { zipCode = "54703" };
        zipModel = _zipApiService.GetLatLong(new ZipModelRoot { ZipCode = preferenceModel.zipCode });
        
        IEnumerable<Reading> sensorReadings = new List<Reading>();
        var sensorReadingsTask = Task.FromResult(sensorReadings);
        var weatherReadingsTask = Task.FromResult((IEnumerable<Reading>?)null);
        var thresholdsTask = Task.FromResult((IEnumerable<Threshold>?)null);

        Task<IEnumerable<WeatherModel>> weatherForecastModelTask = null;
        var readings = await _readingByZipService.Get(preferenceModel.zipCode);
        if (readings != null)
        {
            weatherForecastModelTask = Task.FromResult(JsonSerializer.Deserialize<IEnumerable<WeatherModel>>(readings.WeatherDataSerialized));
        }

        if (readings is null || readings?.LastUpdatedUtc < DateTime.UtcNow.AddMinutes(30))
        {
            weatherForecastModelTask = _weatherForecastService.GetWeatherForecast(zipModel);
            var stuff = JsonSerializer.Serialize(await weatherForecastModelTask);
            await _readingByZipService.Upsert(preferenceModel.zipCode, stuff);
        }
        
        var weatherForecastWithZoneTask = Task.FromResult((await weatherForecastModelTask).Select(w => new WeatherModelWithZone()
        {
            CloudPercentage = w.CloudPercentage,
            DateTimeUtc = w.DateTimeUtc,
            FeelsLikeInCelcius = w.FeelsLikeInCelcius,
            Humidity = w.Humidity,
            InTheZone = false,
            RainfallInCm = w.RainfallInCm,
            SnowfallInCm = w.SnowfallInCm,
            TemperatureInCelcius = w.TemperatureInCelcius,
            WindDirection = w.WindDirection,
            WindGust = w.WindGust,
            WindSpeed = w.WindSpeed,
        } ));
        // var weatherForecastWithZoneTask = GetWeatherForecastWithZone(
        //     weatherForecastModelTask,
        //     thresholdsTask);
        //var weatherForecastWithZoneTask = Task.FromResult((IEnumerable<WeatherModelWithZone>?)null);
        var model = new IndexViewModel(
            sensorReadingsTask,
            weatherReadingsTask,
            thresholdsTask,
            weatherForecastWithZoneTask,
            Task.FromResult<DateTime?>(null),
            preferenceModel
        );
        // var nextZoneChangeTask = GetNextZoneChange(model);
        // model = model with { NextZoneChangeTask = nextZoneChangeTask };
        // await model.NextZoneChangeTask;
        model = model with { NextZoneChangeTask = Task.FromResult((DateTime?)null) };
        return View(model);
    }

    // public async Task<IActionResult> Index(IndexPreferenceModel preferenceModel)
    // {
    //     //var sensorReadingsTask = (Task<IEnumerable<Reading>>)_repositoryReadingService.GetLastNReadingAsync(ReadingTypeEnum.Sensor, 5);
    //     var zipModel = Task.FromResult(new ZipModelRoot());
    //     if (!string.IsNullOrWhiteSpace(preferenceModel.zipCode))
    //     {
    //         zipModel = _zipApiService.GetLatLong(new ZipModelRoot { ZipCode = preferenceModel.zipCode });
    //     }
    //     IEnumerable<Reading> sensorReadings = new List<Reading>();
    //     var sensorReadingsTask = Task.FromResult(sensorReadings);
    //     var weatherReadingsTask = _repositoryReadingService.GetLastNReadingAsync(ReadingTypeEnum.Weather, 5);
    //     var thresholdsTask = _repositoryThresholdService.GetThresholds();

    //     Task<IEnumerable<WeatherModel>> weatherForecastModelTask;
    //     weatherForecastModelTask = _weatherForecastService.GetWeatherForecast(zipModel);
    //     var weatherForecastWithZoneTask = GetWeatherForecastWithZone(
    //         weatherForecastModelTask,
    //         thresholdsTask);
    //     var model = new IndexViewModel(
    //         sensorReadingsTask,
    //         weatherReadingsTask,
    //         thresholdsTask,
    //         weatherForecastWithZoneTask,
    //         Task.FromResult<DateTime?>(null),
    //         preferenceModel
    //     );
    //     var nextZoneChangeTask = GetNextZoneChange(model);
    //     model = model with { NextZoneChangeTask = nextZoneChangeTask };
    //     await model.NextZoneChangeTask;
    //     return View(model);
    // }

    private async Task<IEnumerable<WeatherModelWithZone>> GetWeatherForecastWithZone(
        Task<IEnumerable<WeatherModel>> weatherForecastModel,
        Task<IEnumerable<Threshold>> thresholdsTask)
    {
        var weatherForecastWithZoneModel = new List<WeatherModelWithZone>();
        foreach (var forecast in await weatherForecastModel)
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
                WindDirection = forecast.WindDirection,
                WindSpeed = forecast.WindSpeed,
                WindGust = forecast.WindGust,
            });
        }
        return weatherForecastWithZoneModel;
    }

    private async Task<DateTime?> GetNextZoneChange(IndexViewModel model)
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
