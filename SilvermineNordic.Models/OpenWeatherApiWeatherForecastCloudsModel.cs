using System.Text.Json.Serialization;

namespace SilvermineNordic.Models;

public class OpenWeatherApiWeatherForecastCloudsModel 
{
    [JsonPropertyName("all")]
    public int CloudPercentage { get; set; }
}