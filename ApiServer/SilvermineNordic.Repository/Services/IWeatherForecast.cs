using SilvermineNordic.Repository.Models;

namespace SilvermineNordic.Repository.Services
{
    public interface IWeatherForecast
    {
        Task<IEnumerable<WeatherModel>> GetWeatherForecast();
        Task<DateTime?> GetNextZoneChange(IEnumerable<Threshold> thresholds, bool inTheZone);
        Task<WeatherModel> GetCurrentWeather();
    }
}
