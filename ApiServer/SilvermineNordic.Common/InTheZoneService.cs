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

        public static string GenerateZoneChangeMessage(bool lastSensorZone, bool currentSensorZone, bool lastWeatherZone, bool currentWeatherZone)
        {
            // Both Agree
            if (currentSensorZone && currentWeatherZone)
            {
                return "Sensor and Weather say it is now snow making time!";
            }
            else if (!currentSensorZone && !currentWeatherZone)
            {
                return "Sensor and Weather say snow making time is done.";
            }

            // They used to agree, now they don't
            else if (!lastSensorZone && !lastWeatherZone && currentSensorZone && !currentWeatherZone)
            {
                return "Sensor says it's time to make snow, Weather says not yet.";
            }
            else if (!lastSensorZone && !lastWeatherZone && !currentSensorZone && currentWeatherZone)
            {
                return "Weather says it's time to make snow, Sensor says not yet.";
            }
            else if (lastSensorZone && lastWeatherZone && currentSensorZone && !currentWeatherZone)
            {
                return "Weather says snow making time is done, Sensor says not yet.";
            }
            else if (lastSensorZone && lastWeatherZone && !currentSensorZone && currentWeatherZone)
            {
                return "Sensor says snow making time is done, Weather says not yet.";
            }

            // They didn't agree, but now they do
            else if (!lastSensorZone && lastWeatherZone && !currentSensorZone && !currentWeatherZone)
            {
                return "Weather agrees with Sensor that snow should not be made.";
            }
            else if (lastSensorZone && !lastWeatherZone && !currentSensorZone && !currentWeatherZone)
            {
                return "Sensor agrees with Weather that snow should not be made.";
            }
            else if (!lastSensorZone && lastWeatherZone && currentSensorZone && currentWeatherZone)
            {
                return "Sensor agrees with Weather that snow can now be made.";
            }
            else if (lastSensorZone && !lastWeatherZone && currentSensorZone && currentWeatherZone)
            {
                return "Weather agrees with Sensor that snow can now be made.";
            }

            // Weird situations
            else if ((lastSensorZone && !lastWeatherZone && !currentSensorZone && currentWeatherZone)
                || (!lastSensorZone && lastWeatherZone && currentSensorZone && !currentWeatherZone))
            {
                return "Sensor and Weather flipped. Still in zone.";
            }

            else
            {
                return "";
            }
        }
    }
}