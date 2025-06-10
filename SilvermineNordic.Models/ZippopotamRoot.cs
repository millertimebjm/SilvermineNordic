using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Place
    {
    [JsonPropertyName("place name")]
        public string PlaceName { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public string Latitude { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;

        [JsonPropertyName("state abbreviation")]
        public string StateAbbreviation { get; set; } = string.Empty;
    }

    public class ZippopotamRoot
    {
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("country abbreviation")]
        public string CountryAbbreviation { get; set; } = string.Empty;

        [JsonPropertyName("post code")]
        public string PostCode { get; set; } = string.Empty;
        public List<Place> Places { get; set; } = new List<Place>();
    }

