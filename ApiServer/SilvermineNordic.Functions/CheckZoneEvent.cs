using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SilvermineNordic.Repository.Services;
using SilvermineNordic.Models;
using SilvermineNordic.Common;
using SilvermineNordic.Repository;

namespace SilvermineNordic.Functions
{
    public class CheckZoneEvent
    {
        private readonly IRepositorySensorReading _sensorReadingService;
        private readonly IRepositoryThreshold _thresholdService;
        private readonly IWeatherForecast _weatherForecastService;
        private readonly ISms _smsService;
        private readonly ISilvermineNordicConfiguration _configurationService;

        public CheckZoneEvent(
            IRepositorySensorReading sensorReadingService, 
            IRepositoryThreshold thresholdService,
            IWeatherForecast weatherForecastService,
            ISms smsService,
            ISilvermineNordicConfiguration conigurationService)
        {
            _sensorReadingService = sensorReadingService;
            _thresholdService = thresholdService;
            _weatherForecastService = weatherForecastService;
            _smsService = smsService;
            _configurationService = conigurationService;
        }

        [FunctionName("CheckZoneEvent")]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow} UTC");
            var lastTwoWeatherReading = await _sensorReadingService.GetLastNReadingAsync(SensorReadingTypeEnum.Weather, 2);
            var lastTwoSensorReading = await _sensorReadingService.GetLastNReadingAsync(SensorReadingTypeEnum.Sensor, 2);
            var thresholdData = await _thresholdService.GetThresholds();

            //await VerifySensorIntegrity(lastTwoWeatherReading);
            //await VerifyWeatherIntegrity(lastTwoSensorReading);
            //await VerifyThresholdIntegrity(thresholdData);

            var lastSensorZone = InTheZoneService.IsInZone(thresholdData, lastTwoSensorReading.Last().TemperatureInCelcius, lastTwoSensorReading.Last().Humidity);
            var currentSensorZone = InTheZoneService.IsInZone(thresholdData, lastTwoSensorReading.First().TemperatureInCelcius, lastTwoSensorReading.First().Humidity);

            var lastWeatherZone = InTheZoneService.IsInZone(thresholdData, lastTwoWeatherReading.Last().TemperatureInCelcius, lastTwoWeatherReading.Last().Humidity);
            var currentWeatherZone = InTheZoneService.IsInZone(thresholdData, lastTwoWeatherReading.First().TemperatureInCelcius, lastTwoWeatherReading.First().Humidity);

            if (lastSensorZone != currentSensorZone || lastWeatherZone != currentWeatherZone)
            {
                var message = InTheZoneService.GenerateZoneChangeMessage(lastSensorZone, currentSensorZone, lastWeatherZone, currentWeatherZone);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    var nextZoneChange = await _weatherForecastService.GetNextZoneChange(thresholdData, currentSensorZone || currentWeatherZone);
                    if (nextZoneChange != null)
                    {
                        message += $" Next change forecasted for {nextZoneChange.Value.ToShortDateString() ?? ""} {nextZoneChange.Value.ToShortTimeString() ?? ""} UTC";
                    }
                    else
                    {
                        message += $" No further Zone Change forecasted.";
                    }
                    log.LogInformation("Sending notification: " + message);
                    var phoneNumbers = _configurationService.GetZoneNotificationPhoneNumbers();
                    var validPhoneNumbers = phoneNumbers.Split(",").Where(_ => PhoneNumberService.ValidatePhoneNumber(_)).ToList();
                    foreach (var validPhoneNumber in validPhoneNumbers)
                    {
                        await _smsService.SendSms(validPhoneNumber, message);
                    }
                }
            }
        }
    }
}
