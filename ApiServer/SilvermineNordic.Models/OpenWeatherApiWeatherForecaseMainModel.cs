using System.Xml.Linq;

namespace SilvermineNordic.Models
{
    public class OpenWeatherApiWeatherForecaseMainModel
    {
        public decimal Temp { get; set; }
        public decimal Humidity { get; set; }
        public decimal Temp_Min { get; set; }
        public decimal Temp_Max { get; set; }
    }
}