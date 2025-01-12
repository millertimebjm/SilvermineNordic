using SilvermineNordic.Models;
using System.Threading.Tasks;

namespace SilvermineNordic.Admin.Mvc.Models;

public class IndexModel
{
    public Task<IEnumerable<Reading>> SensorReadingsTask { get; set; } = Task.FromResult(Enumerable.Empty<Reading>());
    public Task<IEnumerable<Reading>> WeatherReadingsTask { get; set; } = Task.FromResult(Enumerable.Empty<Reading>());
    public Task<IEnumerable<Threshold>> ThresholdsTask { get; set; } = Task.FromResult(Enumerable.Empty<Threshold>());
    public Task<IEnumerable<WeatherModelWithZone>> WeatherForecastTask { get; set; } = Task.FromResult(Enumerable.Empty<WeatherModelWithZone>());
    public Task<DateTime?> NextZoneChangeTask { get; set; } = Task.FromResult((DateTime?)null);
}