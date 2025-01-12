namespace SilvermineNordic.Models
{
    public class Threshold
    {
        public int Id { get; set; }
        public decimal TemperatureInCelciusHighThreshold { get; set; }
        public decimal TemperatureInCelciusLowThreshold { get; set; }
        public decimal HumidityHighThreshold { get; set; }
        public decimal HumidityLowThreshold { get; set; }
    }
}
