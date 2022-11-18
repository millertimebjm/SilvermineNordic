using System;

namespace SilvermineNordic.Repository.Models
{
    public class SensorReading
    {
        public int Id { get; set; }
        public decimal TemperatureInCelcius { get; set; }
        public decimal Humidity { get; set; }
        public DateTime DateTimestampUtc { get; set; } = DateTime.UtcNow;
    }
}
