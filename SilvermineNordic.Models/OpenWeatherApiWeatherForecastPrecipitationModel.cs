using System.Text.Json.Serialization;

namespace SilvermineNordic.Models
{
    public class OpenWeatherApiWeatherForecastPrecipitationModel
    {
        [JsonPropertyName("3h")]
        public decimal PrecipitationAmountInCentimeters { get; set; }
    }
}
