using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SilvermineNordic.Common;
using SilvermineNordic.Models;
using SilvermineNordic.Repository;
using SilvermineNordic.Repository.Services;

namespace SilvermineNordic.Functions.Azure
{
    public class ZoneEventCheck
    {
        private readonly ILogger _logger;
        private readonly IRepositoryReading _readingService;
        private readonly IRepositoryThreshold _thresholdService;
        private readonly IWeatherForecast _weatherForecastService;
        private readonly ISms _smsService;
        private readonly ISilvermineNordicConfiguration _configuration;

        public ZoneEventCheck(
            ILoggerFactory loggerFactory,
            IRepositoryReading readingService,
            IRepositoryThreshold thresholdService,
            IWeatherForecast weatherForecastService,
            ISms smsService,
            IOptionsSnapshot<SilvermineNordicConfigurationService> options)
        {
            _logger = loggerFactory.CreateLogger<ZoneEventCheck>();
            _readingService = readingService;
            _thresholdService = thresholdService;
            _weatherForecastService = weatherForecastService;
            _smsService = smsService;
            _configuration = options.Value;
        }

        [Function("ZoneEventCheck")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow} UTC");
            var lastTwoWeatherReadingTask = _readingService.GetLastNReadingAsync(ReadingTypeEnum.Weather, 2);
            var lastTwoSensorReadingTask = _readingService.GetLastNReadingAsync(ReadingTypeEnum.Sensor, 2);
            var thresholdDataTask = _thresholdService.GetThresholds();
            await Task.WhenAll(lastTwoWeatherReadingTask, lastTwoSensorReadingTask, thresholdDataTask);

            var (lastSensorZone,
                currentSensorZone,
                lastWeatherZone,
                currentWeatherZone)
                    = IsInTheZoneChecks(
                        thresholdDataTask.Result,
                        lastTwoSensorReadingTask.Result,
                        lastTwoWeatherReadingTask.Result
                    );

            if (lastSensorZone == currentSensorZone
                && lastWeatherZone == currentWeatherZone)
            {
                _logger.LogInformation("No zone change identified.");
                return;
            }

            _logger.LogInformation("A zone change has been identified.");
            await ZoneChangeNotifyMessage(
                lastSensorZone,
                currentSensorZone,
                lastWeatherZone,
                currentWeatherZone,
                lastTwoSensorReadingTask.Result,
                thresholdDataTask.Result);
        }

        private (bool, bool, bool, bool) IsInTheZoneChecks(
            IEnumerable<Threshold> thresholdData,
            IEnumerable<Reading> lastTwoSensorReading,
            IEnumerable<Reading> lastTwoWeatherReading)
        {
            var lastSensorZone = InTheZoneService.IsInZone(
                thresholdData,
                lastTwoSensorReading.Last().TemperatureInCelcius,
                lastTwoSensorReading.Last().Humidity);
            var currentSensorZone = InTheZoneService.IsInZone(
                thresholdData,
                lastTwoSensorReading.First().TemperatureInCelcius,
                lastTwoSensorReading.First().Humidity);
            var lastWeatherZone = InTheZoneService.IsInZone(
                thresholdData,
                lastTwoWeatherReading.Last().TemperatureInCelcius,
                lastTwoWeatherReading.Last().Humidity);
            var currentWeatherZone = InTheZoneService.IsInZone(
                thresholdData,
                lastTwoWeatherReading.First().TemperatureInCelcius,
                lastTwoWeatherReading.First().Humidity);
            return (lastSensorZone,
                currentSensorZone,
                lastWeatherZone,
                currentWeatherZone);
        }

        private async Task ZoneChangeNotifyMessage(
            bool lastSensorZone,
            bool currentSensorZone,
            bool lastWeatherZone,
            bool currentWeatherZone,
            IEnumerable<Reading> lastTwoSensorReading,
            IEnumerable<Threshold> thresholdData)
        {
            var message = string.Empty;
            if (lastTwoSensorReading
                .Any(_ => _.ReadingDateTimestampUtc > DateTime.UtcNow.AddMinutes(-10)))
            {
                message = InTheZoneService.GenerateZoneChangeSensorWeatherMessage(
                    lastSensorZone,
                    currentSensorZone,
                    lastWeatherZone,
                    currentWeatherZone);
            }
            else
            {
                message = InTheZoneService.GenerateZoneChangeWeatherOnlyMessage(
                    lastWeatherZone,
                    currentWeatherZone);
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var nextZoneChange = await _weatherForecastService.GetNextZoneChange(
                thresholdData,
                currentSensorZone || currentWeatherZone);
            if (nextZoneChange != null)
            {
                var centralTime = CentralTimeService.GetCentralTime(nextZoneChange.Value);
                _logger.LogInformation($"Next change forecasted for {centralTime.ToShortDateString() ?? ""} {centralTime.ToShortTimeString() ?? ""}");
                message += $" Next change forecasted for {centralTime.ToShortDateString() ?? ""} {centralTime.ToShortTimeString() ?? ""}";
            }
            else
            {
                _logger.LogInformation("No further Zone Change forecasted.");
                message += $" No further Zone Change forecasted.";
            }
            _logger.LogInformation("Sending notification: " + message);
            var phoneNumbers = _configuration.GetZoneNotificationPhoneNumbers();
            var validPhoneNumbers = phoneNumbers.Split(",").Where(_ => PhoneNumberService.ValidatePhoneNumber(_)).ToList();
            _logger.LogInformation("Valid Phone Numbers: " + validPhoneNumbers.Count().ToString());
            foreach (var validPhoneNumber in validPhoneNumbers)
            {
                await _smsService.SendSms(validPhoneNumber, message);
            }
            _logger.LogInformation("Message sent to valid phone numbers.");
        }
    }



    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
