using System;

namespace SilvermineNordic.Models
{
    public class Reading
    {
        public int Id { get; set; } = 0;
        public string Type { get; set; } = "";
        public decimal TemperatureInCelcius { get; set; } = 0.0m;
        public decimal Humidity { get; set; } = 0.0m;
        public DateTime DateTimestampUtc { get; set; } = DateTime.UtcNow;
        public DateTime ReadingDateTimestampUtc { get; set; } = DateTime.UtcNow;
        public DateTime InsertedDateTimestampUtc { get; set; } = DateTime.UtcNow;

        public Reading() { }
        public Reading(WeatherModel model)
        {
            Id = 0;
            Type = "Weather";
            TemperatureInCelcius = model.TemperatureInCelcius;
            Humidity = model.Humidity;
            DateTimestampUtc = model.DateTimeUtc;
            ReadingDateTimestampUtc = model.DateTimeUtc;
            InsertedDateTimestampUtc = DateTime.UtcNow;
        }
    }
}
