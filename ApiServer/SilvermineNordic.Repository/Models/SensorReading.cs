namespace SilvermineNordic.Repository.Models
{
    public class SensorReading
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal TemperatureInCelcius { get; set; }
        public decimal Humidity { get; set; }
        public DateTime DateTimestampUtc { get; set; } = DateTime.UtcNow;
        public DateTime ReadingDateTimestampUtc { get; set; }
        public DateTime InsertedDateTimestampUtc { get; set; } = DateTime.UtcNow;
    }
}
