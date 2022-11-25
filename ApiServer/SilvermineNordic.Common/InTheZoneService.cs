using SilvermineNordic.Models;

namespace SilvermineNordic.Common
{
    public static class InTheZoneService
    {
        public static bool IsInZone(IEnumerable<Threshold> thresholdData, decimal temperatureInCelcius, decimal humidity)
        {
            if (thresholdData.Any(_ => _.TemperatureInCelciusLowThreshold <= temperatureInCelcius
                && _.TemperatureInCelciusHighThreshold > temperatureInCelcius
                && _.HumidityLowThreshold <= humidity
                && _.HumidityHighThreshold > humidity))
            {
                return true;
            }
            return false;
        }

        public static decimal? ClosestInTheZoneTemperature(IEnumerable<Threshold> thresholdData, decimal temperatureInCelcius, decimal humidity)
        {
            var threshold = thresholdData.SingleOrDefault(_ => _.HumidityLowThreshold < humidity && _.HumidityHighThreshold > humidity);
            if (threshold == null)
            {
                return null;
            }
            if (temperatureInCelcius > threshold.TemperatureInCelciusHighThreshold)
            {
                return threshold.TemperatureInCelciusHighThreshold;
            }
            if (temperatureInCelcius < threshold.TemperatureInCelciusLowThreshold)
            {
                return threshold.TemperatureInCelciusLowThreshold;
            }
            if (Math.Abs(temperatureInCelcius - threshold.TemperatureInCelciusLowThreshold) > Math.Abs(temperatureInCelcius - threshold.TemperatureInCelciusHighThreshold))
            {
                return threshold.TemperatureInCelciusHighThreshold;
            }
            if (Math.Abs(temperatureInCelcius - threshold.TemperatureInCelciusLowThreshold) < Math.Abs(temperatureInCelcius - threshold.TemperatureInCelciusHighThreshold))
            {
                return threshold.TemperatureInCelciusLowThreshold;
            }

            return null;
        }

        public static DateTime? GetNextZoneChange(IEnumerable<WeatherModel> weatherForecast, IEnumerable<Threshold> thresholds, bool currentZone)
        {
            List<WeatherModel> weatherForecastModels = weatherForecast.ToList();
            int nextWeatherForecastIndex = 0;
            while (nextWeatherForecastIndex < weatherForecastModels.Count() && currentZone == InTheZoneService.IsInZone(thresholds, weatherForecastModels[nextWeatherForecastIndex].TemperatureInCelcius, weatherForecastModels[nextWeatherForecastIndex].Humidity))
            {
                nextWeatherForecastIndex++;
            }
            if (nextWeatherForecastIndex >= weatherForecastModels.Count())
            {
                return (DateTime?)null;
            }
            return weatherForecastModels[nextWeatherForecastIndex].DateTimeUtc;
        }
    }
}