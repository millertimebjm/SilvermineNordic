using Microsoft.Extensions.Options;
using SilvermineNordic.Common;
using SilvermineNordic.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SilvermineNordic.Repository.Services
{
    public class OpenWeatherApiForecastService : IWeatherForecast
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISilvermineNordicConfiguration _configuration;
        public OpenWeatherApiForecastService(
            IHttpClientFactory httpClientFactory,
            IOptionsSnapshot<SilvermineNordicConfigurationService> options)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = options.Value;
        }

        public async Task<IEnumerable<WeatherModel>> GetWeatherForecast(Task<ZipModelRoot> zipModelTask)
        {
            var zipModel = await zipModelTask;
            if (zipModel.FirstPlace is null || (zipModel.FirstPlace?.Latitude == "" && zipModel.FirstPlace?.Longitude == ""))
            {
                return await GetWeatherForecast();
            }
            return await GetWeatherForecastInternal(zipModel.FirstPlace!.Latitude, zipModel.FirstPlace!.Longitude);
        }

        private async Task<IEnumerable<WeatherModel>> GetWeatherForecastInternal(string lat, string lon)
        {
            //https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={API key}
            var url = $"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&appid={_configuration.GetOpenWeatherApiKey()}&mode=json&units=metric";
            using var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);
            var data = await response.Content.ReadAsStringAsync();
            var openApiWeatherModel = JsonSerializer.Deserialize<OpenWeatherApiWeatherForecastRoot>(
                data,
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            //var openApiWeatherModel = await client.GetFromJsonAsync<OpenWeatherApiWeatherForecastRoot>(url);
            var models = new List<WeatherModel>();
            foreach (var forecast in openApiWeatherModel?.List ?? new List<OpenWeatherApiWeatherForecastList>())
            {
                models.Add(new WeatherModel()
                {
                    DateTimeUtc = forecast.DateTimeUtc ?? DateTime.MinValue,
                    TemperatureInCelcius = Convert.ToDecimal(forecast.Main.Temp),
                    FeelsLikeInCelcius = Convert.ToDecimal(forecast.Main.FeelsLike),
                    Humidity = forecast.Main.Humidity,
                    SnowfallInCm = forecast.Snow?.PrecipitationAmountInCentimeters ?? 0,
                    RainfallInCm = forecast.Rain?.PrecipitationAmountInCentimeters ?? 0,
                    CloudPercentage = forecast.Clouds.CloudPercentage,
                    WindSpeed = Convert.ToDecimal(forecast.Wind.Speed),
                    WindGust = Convert.ToDecimal(forecast.Wind.Gust),
                    WindDirection = forecast.Wind.Deg
                });
            }
            return models;
        }

        public async Task<IEnumerable<WeatherModel>> GetWeatherForecast()
        {
            return await GetWeatherForecastInternal(
                "44.772712650825966", "-91.58243961934646"
            );
        }

        public Task<WeatherModel> GetCurrentWeather()
        {
            throw new NotImplementedException();
            // if (_configuration.GetOpenWeatherApiKey() == null)
            //     throw new ArgumentNullException("OpenWeatherApiKey");

            // var url = $"https://api.openweathermap.org/data/2.5/weather?lat=44.772712650825966&lon=-91.58243961934646&appid={_configuration.GetOpenWeatherApiKey()}&mode=json&units=metric";
            // using var client = _httpClientFactory.CreateClient();
            // var openApiWeatherModel = await client.GetFromJsonAsync<OpenWeatherApiCurrentWeatherModel>(url);
            // var model = new WeatherModel()
            // {
            //     DateTimeUtc = DateTime.UtcNow,
            //     TemperatureInCelcius = openApiWeatherModel?.Main.Temp ?? 0.0m,
            //     Humidity = openApiWeatherModel?.Main.Humidity ?? 0.0m,
            // };
            // return model;
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
