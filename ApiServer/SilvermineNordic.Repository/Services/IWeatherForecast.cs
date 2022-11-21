using SilvermineNordic.Repository.Models;

namespace SilvermineNordic.Repository.Services
{
    public interface IWeatherForecast
    {
        Task<WeatherForecastListModel> GetWeatherForecast();
        Task<DateTime?> GetNextZoneChange(IEnumerable<Threshold> thresholds, bool inTheZone);
        Task<CurrentWeatherModel> GetCurrentWeather();
    }
}
