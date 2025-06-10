
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SilvermineNordic.Models;

public class ZipModelRoot
{
    [JsonPropertyName("post code")]
    public string ZipCode { get; set; } = string.Empty;

    public List<Place> Places { get; set; } = new List<Place>();
    public Place? FirstPlace
    {
        get
        {
            return Places.FirstOrDefault();
        }
        set
        {
            if (value != null) Places.Add(value);
        }
    }
}

public class Place
{
    [JsonPropertyName("place name")]
    public string City { get; set; } = string.Empty;
    [JsonPropertyName("state abbreviation")]
    public string StateCode { get; set; } = string.Empty;
    [JsonPropertyName("state")]
    public string StateName { get; set; } = string.Empty;
    public string Latitude { get; set; } = string.Empty;
    public string Longitude { get; set; } = string.Empty;
}