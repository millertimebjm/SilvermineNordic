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

        [Function("CheckZoneChange")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] MyInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow} UTC");
            var lastTwoWeatherReadingTask = _readingService.GetLastNReadingAsync(ReadingTypeEnum.Weather, 2);
            var lastTwoSensorReadingTask = _readingService.GetLastNReadingAsync(ReadingTypeEnum.Sensor, 2);
            var thresholdDataTask = _thresholdService.GetThresholds();
            await Task.WhenAll(lastTwoWeatherReadingTask, lastTwoSensorReadingTask, thresholdDataTask);


            var lastSensorZone = InTheZoneService.IsInZone(
                thresholdDataTask.Result,
                lastTwoSensorReadingTask.Result.Last().TemperatureInCelcius,
                lastTwoSensorReadingTask.Result.Last().Humidity);
            var currentSensorZone = InTheZoneService.IsInZone(
                thresholdDataTask.Result,
                lastTwoSensorReadingTask.Result.First().TemperatureInCelcius,
                lastTwoSensorReadingTask.Result.First().Humidity);

            var lastWeatherZone = InTheZoneService.IsInZone(
                thresholdDataTask.Result,
                lastTwoWeatherReadingTask.Result.Last().TemperatureInCelcius,
                lastTwoWeatherReadingTask.Result.Last().Humidity);
            var currentWeatherZone = InTheZoneService.IsInZone(
                thresholdDataTask.Result,
                lastTwoWeatherReadingTask.Result.First().TemperatureInCelcius,
                lastTwoWeatherReadingTask.Result.First().Humidity);

            if (lastSensorZone == currentSensorZone && lastWeatherZone == currentWeatherZone)
            {
                _logger.LogInformation("No zone change identified.");
                return;
            }

            _logger.LogInformation("A zone change has been identified.");

            var message = string.Empty;
            if (lastTwoSensorReadingTask.Result
                .Any(_ => _.ReadingDateTimestampUtc > DateTime.UtcNow.AddMinutes(-10)))
            {
                message = InTheZoneService.GenerateZoneChangeSensorWeatherMessage(lastSensorZone, currentSensorZone, lastWeatherZone, currentWeatherZone);
            }
            else
            {
                message = InTheZoneService.GenerateZoneChangeWeatherOnlyMessage(lastWeatherZone, currentWeatherZone);
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            await Notify(
                message,
                currentSensorZone,
                currentWeatherZone,
                thresholdDataTask);
        }

        private async Task Notify(
            string message,
            bool currentSensorZone,
            bool currentWeatherZone,
            Task<IEnumerable<Threshold>> thresholdDataTask)
        {
            var nextZoneChange = await _weatherForecastService.GetNextZoneChange(
                    thresholdDataTask.Result,
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
