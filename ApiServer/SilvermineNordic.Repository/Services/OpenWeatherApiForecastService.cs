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
            var url = $"https://api.openweathermap.org/data/2.5/forecast?lat=44.772712650825966&lon=91.58243961934646&appid={_configuration.GetOpenWeatherApiKey()}&mode=json&units=metric";
            using var client = _httpClientFactory.CreateClient();
            var forecastListModel = await client.GetFromJsonAsync<WeatherForecastListModel>(url);
            return forecastListModel;
        }
    }
}
