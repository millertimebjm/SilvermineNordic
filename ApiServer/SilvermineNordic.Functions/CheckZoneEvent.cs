using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SilvermineNordic.Repository.Services;
using SilvermineNordic.Repository;
using SilvermineNordic.Common;
using SilvermineNordic.Models;

namespace SilvermineNordic.Functions
{
    public class CheckZoneEvent
    {
        private readonly ILogger _logger;
        private readonly IRepositorySensorReading _sensorReadingService;
        private readonly IRepositoryThreshold _thresholdService;
        private readonly IWeatherForecast _weatherForecastService;
        private readonly ISms _smsService;
        private readonly ISilvermineNordicConfiguration _configurationService;

        public CheckZoneEvent(ILoggerFactory loggerFactory,
            IRepositorySensorReading sensorReadingService,
            IRepositoryThreshold thresholdService,
            IWeatherForecast weatherForecastService,
            ISms smsService,
            ISilvermineNordicConfiguration conigurationService)
        {
            _logger = loggerFactory.CreateLogger<CheckZoneEvent>();
            _sensorReadingService = sensorReadingService;
            _thresholdService = thresholdService;
            _weatherForecastService = weatherForecastService;
            _smsService = smsService;
            _configurationService = conigurationService;
        }

        [Function("CheckZoneEvent")]
        public async Task RunAsync([TimerTrigger("0 */5 * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow} UTC");
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

            if (lastSensorZone == currentSensorZone && lastWeatherZone == currentWeatherZone)
            {
                return;
            }

            _logger.LogInformation("A zone change has been identified.");

            var message = string.Empty;
            if (lastTwoSensorReading.Any(_ => _.ReadingDateTimestampUtc > DateTime.UtcNow.AddMinutes(-10)))
            {
                message = InTheZoneService.GenerateZoneChangeSensorWeatherMessage(lastSensorZone, currentSensorZone, lastWeatherZone, currentWeatherZone);
            }
            else
            {
                message = InTheZoneService.GenerateZoneChangeWeatherMessage(lastWeatherZone, currentWeatherZone);
            }
            
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var nextZoneChange = await _weatherForecastService.GetNextZoneChange(thresholdData, currentSensorZone || currentWeatherZone);
            if (nextZoneChange != null)
            {
                var centralTime = CentralTimeService.GetCentralTime(nextZoneChange.Value);
                message += $" Next change forecasted for {centralTime.ToShortDateString() ?? ""} {centralTime.ToShortTimeString() ?? ""}";
            }
            else
            {
                message += $" No further Zone Change forecasted.";
            }
            _logger.LogInformation("Sending notification: " + message);
            var phoneNumbers = _configurationService.GetZoneNotificationPhoneNumbers();
            var validPhoneNumbers = phoneNumbers.Split(",").Where(_ => PhoneNumberService.ValidatePhoneNumber(_)).ToList();
            _logger.LogInformation("Valid Phone Numbers: " + validPhoneNumbers.Count().ToString());
            foreach (var validPhoneNumber in validPhoneNumbers)
            {
                await _smsService.SendSms(validPhoneNumber, message);
            }
            _logger.LogInformation("Message sent to valid phone numbers.");
        }
    }
}
