using SilvermineNordic.Models;
using SilvermineNordic.Common;

namespace SilvermineNordic.Tests
{
    public class Tests
    {
        static object[] SensorReadings =
        {
            new object[] {
                new SensorReading() { TemperatureInCelcius = 3.0m, Humidity = 20.0m }, true // In The Zone
            },
            new object[] {
                new SensorReading() { TemperatureInCelcius = 1.0m, Humidity = 15.0m }, true // Low Edge In The Zone
            },
            new object[] {
                new SensorReading() { TemperatureInCelcius = 10.0m, Humidity = 35.0m }, false // High Edge Out Of The Zone
            },
            new object[] {
                new SensorReading() { TemperatureInCelcius = 2.5m, Humidity = 30.0m }, false // High Humidity
            },
            new object[] {
                new SensorReading() { TemperatureInCelcius = 7.5m, Humidity = 10.0m }, false // LowHumidity
            },
            new object[] {
                new SensorReading() { TemperatureInCelcius = 15m, Humidity = 30.0m }, false // HighTemperature
            },
            new object[] {
                new SensorReading() { TemperatureInCelcius = 0.0m, Humidity = 20.0m }, false // LowTemperature
            },
            new object[] {
                new SensorReading() { TemperatureInCelcius = 15.0m, Humidity = 40.0m }, false // High
            },
            new object[] {
                new SensorReading() { TemperatureInCelcius = 0.0m, Humidity = 10.0m }, false // Low
            },
            new object[] {
                new SensorReading() { TemperatureInCelcius = 7.5m, Humidity = 20.0m }, false // HighTemperatureLowHumidity
            },
            new object[] {
                new SensorReading() { TemperatureInCelcius = 2.5m, Humidity = 30.0m }, false // LowTemperatureHighHumidity
            },
        };



        private readonly List<Threshold> SensorThresholds = new List<Threshold>()
        {
            new Threshold()
            {
                Id = 0,
                TemperatureInCelciusLowThreshold = 1.0m,
                TemperatureInCelciusHighThreshold = 5.0m,
                HumidityLowThreshold = 15.0m,
                HumidityHighThreshold = 25.0m,
            },
            new Threshold()
            {
                Id = 0,
                TemperatureInCelciusLowThreshold = 5.0m,
                TemperatureInCelciusHighThreshold = 10.0m,
                HumidityLowThreshold = 25.0m,
                HumidityHighThreshold = 35.0m,
            },
        };

        [Theory]
        [TestCaseSource(nameof(SensorReadings))]
        public void InTheZoneTest(SensorReading sensorReading, bool inTheZone)
        {
            var inTheZoneCalculated = InTheZoneService.IsInZone(SensorThresholds, sensorReading.TemperatureInCelcius, sensorReading.Humidity);
            Assert.AreEqual(inTheZoneCalculated, inTheZone);
        }
    }
}