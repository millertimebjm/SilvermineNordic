using System;

namespace SilvermineNordic.Models
{
    public class Reading
    {
        public int Id { get; set; } = 0;
        public string Type { get; set; } = "";
        public decimal TemperatureInCelcius { get; set; } = 0.0m;
        public decimal Humidity { get; set; } = 0.0m;
        public DateTime DateTimeUtc { get; set; } = DateTime.UtcNow;

        public Reading() { }
        public Reading(WeatherModel model)
        {
            Id = 0;
            Type = "Weather";
            TemperatureInCelcius = model.TemperatureInCelcius;
            Humidity = model.Humidity;
            DateTimeUtc = model.DateTimeUtc;
        }
    }
}
