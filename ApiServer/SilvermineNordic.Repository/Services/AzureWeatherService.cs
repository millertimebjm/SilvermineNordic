using SilvermineNordic.Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SilvermineNordic.Repository.Services
{
    internal class AzureWeatherService : IWeatherForecast
    {
        public AzureWeatherService()
        {

        }

        public Task<WeatherModel> GetCurrentWeather()
        {
            // https://atlas.microsoft.com/weather/currentConditions/json?api-version=1.1&query=44.772712650825966,-91.58243961934646&subscription-key=
            throw new NotImplementedException();
        }

        public Task<DateTime?> GetNextZoneChange(IEnumerable<Threshold> thresholds, bool inTheZone)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WeatherModel>> GetWeatherForecast()
        {
            throw new NotImplementedException();
        }
    }
}
