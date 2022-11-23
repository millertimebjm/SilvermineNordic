using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SilvermineNordic.Repository.Services;
using SilvermineNordic.Repository.Models;
using Twilio.TwiML.Messaging;
using System.Collections;
using System.Collections.Generic;

namespace SilvermineNordic.Functions
{
    public class CheckZoneEvent
    {
        private readonly IRepositorySensorReading _sensorReadingService;
        private readonly IRepositoryThreshold _thresholdService;
        private readonly IWeatherForecast _weatherForecastService;
        private readonly ISms _smsService;

        public CheckZoneEvent(
            IRepositorySensorReading sensorReadingService, 
            IRepositoryThreshold thresholdService,
            IWeatherForecast weatherForecastService,
            ISms smsService)
        {
            _sensorReadingService = sensorReadingService;
            _thresholdService = thresholdService;
            _weatherForecastService = weatherForecastService;
            _smsService = smsService;
        }

        [FunctionName("CheckZoneEvent")]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow} UTC");
            var lastTwoWeatherReading = await _sensorReadingService.GetLastNReadingAsync(SensorReadingTypeEnum.Weather, 2);
            var lastTwoSensorReading = await _sensorReadingService.GetLastNReadingAsync(SensorReadingTypeEnum.Sensor, 2);
            var thresholdData = await _thresholdService.GetThresholds();

            await VerifySensorIntegrity(lastTwoWeatherReading);
            await VerifyWeatherIntegrity(lastTwoSensorReading);
            await VerifyThresholdIntegrity(thresholdData);

            var lastSensorZone = InTheZoneService.IsInZone(thresholdData, lastTwoSensorReading.Last().TemperatureInCelcius, lastTwoSensorReading.Last().Humidity);
            var currentSensorZone = InTheZoneService.IsInZone(thresholdData, lastTwoSensorReading.First().TemperatureInCelcius, lastTwoSensorReading.First().Humidity);

            var lastWeatherZone = InTheZoneService.IsInZone(thresholdData, lastTwoWeatherReading.Last().TemperatureInCelcius, lastTwoWeatherReading.Last().Humidity);
            var currentWeatherZone = InTheZoneService.IsInZone(thresholdData, lastTwoWeatherReading.First().TemperatureInCelcius, lastTwoWeatherReading.First().Humidity);

            if (lastSensorZone != currentSensorZone || lastWeatherZone != currentWeatherZone)
            {
                var message = GenerateMessage(lastSensorZone, currentSensorZone, lastWeatherZone, currentWeatherZone);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    var nextZoneChange = await _weatherForecastService.GetNextZoneChange(thresholdData, currentSensorZone || currentWeatherZone);
                    message += $" Next change forecasted for {nextZoneChange.Value.ToShortDateString()} {nextZoneChange.Value.ToShortTimeString()} UTC";
                    log.LogInformation(message);
                    await _smsService.SendSms("+17155239481", message);
                    await _smsService.SendSms("+17155792999", message);
                }
            }
        }

        private async Task VerifyThresholdIntegrity(IEnumerable<Threshold> thresholds)
        {
            //if (thresholds.Any(_ => _.TemperatureInCelciusLowThreshold > _.TemperatureInCelciusHighThreshold
            //    || _.HumidityLowThreshold > _.HumidityHighThreshold))
            //{
            //    await _smsService.SendSms("+17155239481", $"Error in ThresholdIntegrity.");
            //}
        }

        private async Task VerifyWeatherIntegrity(IEnumerable<SensorReading> weatherReadings)
        {
            if (weatherReadings.Last().InsertedDateTimestampUtc < DateTime.UtcNow.AddMinutes(-8) 
                && weatherReadings.Last().InsertedDateTimestampUtc > DateTime.UtcNow.AddMinutes(-13))
            {
                await _smsService.SendSms("+17155239481", $"Error in WeatherIntegrity. Last Weather Reading from {weatherReadings.Last().InsertedDateTimestampUtc.ToShortDateString()} {weatherReadings.Last().InsertedDateTimestampUtc.ToShortTimeString()} UTC");
            }
        }

        private async Task VerifySensorIntegrity(IEnumerable<SensorReading> sensorReadings)
        {
            if (sensorReadings.Last().InsertedDateTimestampUtc < DateTime.UtcNow.AddMinutes(-8)
                && sensorReadings.Last().InsertedDateTimestampUtc > DateTime.UtcNow.AddMinutes(-13))
            {
                await _smsService.SendSms("+17155239481", $"Error in SensorIntegrity. Last Sensor Reading from {sensorReadings.Last().InsertedDateTimestampUtc.ToShortDateString()} {sensorReadings.Last().InsertedDateTimestampUtc.ToShortTimeString()} UTC");
            }
        }

        private static string GenerateMessage(bool lastSensorZone, bool currentSensorZone, bool lastWeatherZone, bool currentWeatherZone)
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

            return "";
        }
    }
}
