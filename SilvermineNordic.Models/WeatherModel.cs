
using System;

namespace SilvermineNordic.Models
{
    public class WeatherModel
    {
        public DateTime DateTimeUtc { get; set; }
        public decimal TemperatureInCelcius { get; set; }
        public decimal Humidity { get; set; }
        public decimal SnowfallInCm { get; set; }
    }
}
