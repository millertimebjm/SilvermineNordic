using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SilvermineNordic.Models;

public class ReadingByZip
{
    public int Id {get; set;}
    public string Zip {get; set;} = string.Empty;
    public DateTime LastLookupUtc {get; set;} = DateTime.UtcNow;
    public DateTime LastUpdatedUtc {get; set;}= DateTime.UtcNow;
    public string WeatherDataSerialized { get; set;} = string.Empty;
}