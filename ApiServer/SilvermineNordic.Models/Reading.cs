using System;

namespace SilvermineNordic.Models
{
    public class Reading
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal TemperatureInCelcius { get; set; }
        public decimal Humidity { get; set; }
        public DateTime DateTimestampUtc { get; set; } = DateTime.UtcNow;
        public DateTime ReadingDateTimestampUtc { get; set; }
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
