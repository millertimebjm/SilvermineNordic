
using System.Collections.Generic;

namespace SilvermineNordic.Models
{
    public class OpenWeatherApiWeatherForecastListModel
    {
        public string Cod { get; set; } = string.Empty;
        public int Message { get; set; } = 0;
        public int Count { get; set; } = 0;
        public List<OpenWeatherApiWeatherForecastModel> List { get; set; } = new List<OpenWeatherApiWeatherForecastModel>();
    }
}
