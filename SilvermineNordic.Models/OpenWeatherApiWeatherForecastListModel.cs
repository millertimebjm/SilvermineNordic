using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SilvermineNordic.Models;

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Coord Coord { get; set; } = new();
    public string Country { get; set; } = string.Empty;
    public int Population { get; set; }
    public int Timezone { get; set; }
    public int Sunrise { get; set; }
    public int Sunset { get; set; }
}

public class Clouds
{
    [JsonPropertyName("All")]
    public int CloudPercentage { get; set; }
    
}

public class Coord
{
    public double Lat { get; set; }
    public double Lon { get; set; }
}

public class OpenWeatherApiWeatherForecastList
{
    public int Dt { get; set; }
    public DateTime? DateTimeUtc
    {
        get
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Dt);
        }
    }
    public Main Main { get; set; } = new();
    public List<Weather> Weather { get; set; } = new();
    public Clouds Clouds { get; set; } = new();
    public Wind Wind { get; set; } = new();
    public int Visibility { get; set; }
    public double Pop { get; set; }
    public Precipitation? Rain { get; set; }
    public Precipitation? Snow { get; set; }
    public Sys Sys { get; set; } = new();
    public string DtTxt { get; set; } = string.Empty;
}

public class Main
{
    public double Temp { get; set; }
    [JsonPropertyName("Feels_Like")]
    public double FeelsLike { get; set; }
    public double TempMin { get; set; }
    public double TempMax { get; set; }
    public int Pressure { get; set; }
    public int SeaLevel { get; set; }
    public int GrndLevel { get; set; }
    public int Humidity { get; set; }
    public double TempKf { get; set; }
}

public class Precipitation
{
    [JsonPropertyName("3h")]
    //public double _3h { get; set; }
    public decimal PrecipitationAmountInCentimeters { get; set; }
}

public class OpenWeatherApiWeatherForecastRoot
{
    public string Cod { get; set; } = string.Empty;
    public int Message { get; set; }
    public int Cnt { get; set; }
    public List<OpenWeatherApiWeatherForecastList> List { get; set; } = new();
    public City City { get; set; } = new();
}

public class Sys
{
    public string Pod { get; set; } = string.Empty;
}

public class Weather
{
    public int Id { get; set; }
    public string Main { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class Wind
{
    public double Speed { get; set; }
    public int Deg { get; set; }
    public double Gust { get; set; }
}

