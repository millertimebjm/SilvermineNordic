using System.Text.Json.Serialization;

namespace SilvermineNordic.Models
{
    public class OpenWeatherApiWeatherForecastSnowModel
    {
        [JsonPropertyName("3h")]
        public decimal SnowfallAmountInCentimeters { get; set; }
    }
}
