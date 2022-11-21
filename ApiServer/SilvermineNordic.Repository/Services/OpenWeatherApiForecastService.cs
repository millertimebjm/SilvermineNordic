using SilvermineNordic.Repository.Models;
using System.Net.Http.Json;

namespace SilvermineNordic.Repository.Services
{
    public class OpenWeatherApiForecastService : IWeatherForecast
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public OpenWeatherApiForecastService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<WeatherForecastListModel> GetWeatherForecast()
        {
            //https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={API key}

            var url = $"https://api.openweathermap.org/data/2.5/forecast?lat=44.772712650825966&lon=91.58243961934646&appid={_configuration.GetOpenWeatherApiKey()}&mode=json&units=metric";
            using var client = _httpClientFactory.CreateClient();
            var model = await client.GetFromJsonAsync<WeatherForecastListModel>(url);
            return model;
        }

        public async Task<CurrentWeatherModel> GetCurrentWeather()
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?lat=44.772712650825966&lon=91.58243961934646&appid={_configuration.GetOpenWeatherApiKey()}&mode=json&units=metric";
            using var client = _httpClientFactory.CreateClient();
            var model = await client.GetFromJsonAsync<CurrentWeatherModel>(url);
            return model;
        }

        public async Task<DateTime?> GetNextZoneChange(IEnumerable<Threshold> thresholds, bool inTheZone)
        {
            var weatherForecastList = await GetWeatherForecast();
            foreach (var weatherForecast in weatherForecastList.List)
            {
                var newInTheZone = InTheZoneService.IsInZone(thresholds, weatherForecast.Main.Temp, weatherForecast.Main.Humidity);
                if (inTheZone != newInTheZone)
                {
                    return weatherForecast.DateTimeUtc;
                }
            }
            return null;
        }
    }
}
