using SilvermineNordic.Common;
using SilvermineNordic.Models;
using System.Net.Http.Json;

namespace SilvermineNordic.Repository.Services
{
    public class OpenWeatherApiForecastService : IWeatherForecast
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISilvermineNordicConfiguration _configuration;
        public OpenWeatherApiForecastService(IHttpClientFactory httpClientFactory, ISilvermineNordicConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IEnumerable<WeatherModel>> GetWeatherForecast()
        {
            //https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={API key}
            var url = $"https://api.openweathermap.org/data/2.5/forecast?lat=44.772712650825966&lon=-91.58243961934646&appid={_configuration.GetOpenWeatherApiKey()}&mode=json&units=metric";
            using var client = _httpClientFactory.CreateClient();
            var openApiWeatherModel = await client.GetFromJsonAsync<OpenWeatherApiWeatherForecastListModel>(url);
            var models = new List<WeatherModel>();
            foreach (var forecast in openApiWeatherModel.List)
            {
                models.Add(new WeatherModel()
                {
                    DateTimeUtc = forecast.DateTimeUtc.Value,
                    TemperatureInCelcius = forecast.Main.Temp,
                    Humidity = forecast.Main.Humidity,
                });
            }
            return models;
        }

        public async Task<WeatherModel> GetCurrentWeather()
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?lat=44.772712650825966&lon=-91.58243961934646&appid={_configuration.GetOpenWeatherApiKey()}&mode=json&units=metric";
            using var client = _httpClientFactory.CreateClient();
            var openApiWeatherModel = await client.GetFromJsonAsync<OpenWeatherApiCurrentWeatherModel>(url);
            var model = new WeatherModel()
            {
                DateTimeUtc = DateTime.UtcNow,
                TemperatureInCelcius = openApiWeatherModel.Main.Temp,
                Humidity = openApiWeatherModel.Main.Humidity,
            };
            return model;
        }

        public async Task<DateTime?> GetNextZoneChange(IEnumerable<Threshold> thresholds, bool inTheZone)
        {
            var weatherForecastList = await GetWeatherForecast();
            foreach (var weatherForecast in weatherForecastList)
            {
                var newInTheZone = InTheZoneService.IsInZone(thresholds, weatherForecast.TemperatureInCelcius, weatherForecast.Humidity);
                if (inTheZone != newInTheZone)
                {
                    return weatherForecast.DateTimeUtc;
                }
            }
            return DateTime.MaxValue;
        }
    }
}
