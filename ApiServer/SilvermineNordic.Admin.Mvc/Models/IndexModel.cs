using SilvermineNordic.Models;
using System.Threading.Tasks;

namespace SilvermineNordic.Admin.Mvc.Models;

public class IndexModel
{
    public IEnumerable<Reading> SensorReadingsTask { get; set; }
    public IEnumerable<Reading> WeatherReadingsTask { get; set; }
    public IEnumerable<Threshold> ThresholdsTask { get; set; }
    public IEnumerable<WeatherModel> WeatherForecastTask { get; set; }
}