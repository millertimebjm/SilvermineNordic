using SilvermineNordic.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilvermineNordic.Repository.Services
{
    public interface IWeatherForecast
    {
        Task<IEnumerable<WeatherModel>> GetWeatherForecast();
        Task<DateTime?> GetNextZoneChange(IEnumerable<Threshold> thresholds, bool inTheZone);
        Task<WeatherModel> GetCurrentWeather();
    }
}
