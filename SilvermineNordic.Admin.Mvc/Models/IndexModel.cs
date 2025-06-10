using SilvermineNordic.Models;

namespace SilvermineNordic.Admin.Mvc.Models;

public record IndexViewModel(
    Task<IEnumerable<Reading>> SensorReadingsTask,
    Task<IEnumerable<Reading>> WeatherReadingsTask,
    Task<IEnumerable<Threshold>> ThresholdsTask,
    Task<IEnumerable<WeatherModelWithZone>> WeatherForecastTask,
    Task<DateTime?> NextZoneChangeTask);