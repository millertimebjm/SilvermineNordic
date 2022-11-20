using SilvermineNordic.Repository.Models;

namespace SilvermineNordic.Repository.Services
{
    public interface IWeatherForecast
    {
        public Task<WeatherForecastListModel> GetWeatherForecast();
    }
}
