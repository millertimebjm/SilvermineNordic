using SilvermineNordic.Models;
using System.Threading.Tasks;

namespace SilvermineNordic.Admin.Mvc.Models;

public class IndexModel
{
    public Task<IEnumerable<Reading>> SensorReadingsTask { get; set; }
    public Task<IEnumerable<Reading>> WeatherReadingsTask { get; set; }
    public Task<IEnumerable<Threshold>> ThresholdsTask { get; set; }
    public Task<IEnumerable<WeatherModelWithZone>> WeatherForecastTask { get; set; }
    public Task<DateTime?> NextZoneChangeTask { get; set; }
}