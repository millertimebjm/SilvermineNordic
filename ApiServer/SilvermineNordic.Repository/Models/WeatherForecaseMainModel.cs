using System.Xml.Linq;

namespace SilvermineNordic.Repository.Models
{
    public class WeatherForecaseMainModel
    {
        public decimal Temp { get; set; }
        public decimal Humidity { get; set; }
        public decimal Temp_Min { get; set; }
        public decimal Temp_Max { get; set; }
    }
}