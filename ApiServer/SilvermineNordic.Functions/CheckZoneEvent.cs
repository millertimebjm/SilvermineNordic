using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SilvermineNordic.Repository.Services;

namespace SilvermineNordic.Functions
{
    public class CheckZoneEvent
    {
        private readonly IRepositorySensorReading _sensorReadingService;
        private readonly IRepositoryThreshold _thresholdService;
        public CheckZoneEvent(IRepositorySensorReading sensorReadingService, IRepositoryThreshold thresholdService)
        {
            _sensorReadingService = sensorReadingService;
            _thresholdService = thresholdService;
        }

        [FunctionName("CheckZoneEvent")]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var lastTwoWeatherReading = await _sensorReadingService.GetLastTwoWeatherReadingAsync();
            var lastTwoSensorReading = await _sensorReadingService.GetLastTwoSensorReadingAsync();
            var thresholdData = await _thresholdService.GetThresholds();

            var lastSensorZone = InTheZoneService.IsInZone(thresholdData, lastTwoSensorReading.Last().TemperatureInCelcius, lastTwoSensorReading.Last().Humidity);
            var currentSensorZone = InTheZoneService.IsInZone(thresholdData, lastTwoSensorReading.First().TemperatureInCelcius, lastTwoSensorReading.First().Humidity);

            var lastWeatherZone = InTheZoneService.IsInZone(thresholdData, lastTwoWeatherReading.Last().TemperatureInCelcius, lastTwoWeatherReading.Last().Humidity);
            var currentWeatherZone = InTheZoneService.IsInZone(thresholdData, lastTwoWeatherReading.First().TemperatureInCelcius, lastTwoWeatherReading.First().Humidity);

            if (lastSensorZone != currentSensorZone || currentWeatherZone != lastWeatherZone)
            {

            }
        }
    }
}
