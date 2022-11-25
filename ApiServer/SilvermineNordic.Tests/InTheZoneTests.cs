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

        private List<Threshold> thresholds = new List<Threshold>()
            {
                new Threshold()
                {
                    HumidityLowThreshold = 10.0M,
                    HumidityHighThreshold = 20.0M,
                    TemperatureInCelciusLowThreshold = 0.0M,
                    TemperatureInCelciusHighThreshold = 10.0M,
                },
                new Threshold()
                {
                    HumidityLowThreshold = 20.0M,
                    HumidityHighThreshold = 30.0M,
                    TemperatureInCelciusLowThreshold = 5.0M,
                    TemperatureInCelciusHighThreshold = 15.0M,
                },
                new Threshold()
                {
                    HumidityLowThreshold = 30.0M,
                    HumidityHighThreshold = 40.0M,
                    TemperatureInCelciusLowThreshold = 10.0M,
                    TemperatureInCelciusHighThreshold = 20.0M,
                },
            };

        [Test]
        public void GetNextZoneChange_AllLowWeatherModels_False_Null()
        {
            var dateTimeUtcNow = DateTime.UtcNow;
            var weatherModels = new List<WeatherModel>()
            {
                new WeatherModel()
                {
                    TemperatureInCelcius = 2.0M,
                    Humidity = 5.0M,
                    DateTimeUtc = dateTimeUtcNow,
                },
                new WeatherModel()
                {
                    TemperatureInCelcius = 4.0M,
                    Humidity = 7.0M,
                    DateTimeUtc = dateTimeUtcNow.AddMinutes(1),
                },
                new WeatherModel()
                {
                    TemperatureInCelcius = 6.0M,
                    Humidity = 9.0M,
                    DateTimeUtc = dateTimeUtcNow.AddMinutes(2),
                },
            };
            var nextZoneChangeDateTime = InTheZoneService.GetNextZoneChange(weatherModels, thresholds, false);
            Assert.IsNull(nextZoneChangeDateTime);
        }

        [Test]
        public void GetNextZoneChange_AllHighWeatherModels_False_Null()
        {
            var dateTimeUtcNow = DateTime.UtcNow;
            var weatherModels = new List<WeatherModel>()
            {
                new WeatherModel()
                {
                    TemperatureInCelcius = 55.0M,
                    Humidity = 35.0M,
                    DateTimeUtc = dateTimeUtcNow,
                },
                new WeatherModel()
                {
                    TemperatureInCelcius = 45.0M,
                    Humidity = 40.0M,
                    DateTimeUtc = dateTimeUtcNow.AddMinutes(1),
                },
                new WeatherModel()
                {
                    TemperatureInCelcius = 63.0M,
                    Humidity = 92.0M,
                    DateTimeUtc = dateTimeUtcNow.AddMinutes(2),
                },
            };
            var nextZoneChangeDateTime = InTheZoneService.GetNextZoneChange(weatherModels, thresholds, false);
            Assert.IsNull(nextZoneChangeDateTime);
        }

        [Test]
        public void GetNextZoneChange_InRangeWeatherModels_True_Null()
        {
            var dateTimeUtcNow = DateTime.UtcNow;
            var weatherModels = new List<WeatherModel>()
            {
                new WeatherModel()
                {
                    TemperatureInCelcius = 5.0M,
                    Humidity = 15.0M,
                    DateTimeUtc = dateTimeUtcNow,
                },
                new WeatherModel()
                {
                    TemperatureInCelcius = 10.0M,
                    Humidity = 25.0M,
                    DateTimeUtc = dateTimeUtcNow.AddMinutes(1),
                },
                new WeatherModel()
                {
                    TemperatureInCelcius = 15.0M,
                    Humidity = 35.0M,
                    DateTimeUtc = dateTimeUtcNow.AddMinutes(2),
                },
            };
            var nextZoneChangeDateTime = InTheZoneService.GetNextZoneChange(weatherModels, thresholds, true);
            Assert.IsNull(nextZoneChangeDateTime);
        }

        [Test]
        public void GetNextZoneChange_InRangeWeatherModels_False_First()
        {
            var dateTimeUtcNow = DateTime.UtcNow;
            var weatherModels = new List<WeatherModel>()
            {
                new WeatherModel()
                {
                    TemperatureInCelcius = 5.0M,
                    Humidity = 15.0M,
                    DateTimeUtc = dateTimeUtcNow,
                },
                new WeatherModel()
                {
                    TemperatureInCelcius = 10.0M,
                    Humidity = 25.0M,
                    DateTimeUtc = dateTimeUtcNow.AddMinutes(1),
                },
                new WeatherModel()
                {
                    TemperatureInCelcius = 15.0M,
                    Humidity = 35.0M,
                    DateTimeUtc = dateTimeUtcNow.AddMinutes(2),
                },
            };
            var nextZoneChangeDateTime = InTheZoneService.GetNextZoneChange(weatherModels, thresholds, false);
            Assert.AreEqual(nextZoneChangeDateTime, weatherModels.First().DateTimeUtc);
        }

        public void GetNextZoneChange_AllHighWeatherModels_True_First()
        {
            var dateTimeUtcNow = DateTime.UtcNow;
            var weatherModels = new List<WeatherModel>()
            {
                new WeatherModel()
                {
                    TemperatureInCelcius = 55.0M,
                    Humidity = 35.0M,
                    DateTimeUtc = dateTimeUtcNow,
                },
                new WeatherModel()
                {
                    TemperatureInCelcius = 45.0M,
                    Humidity = 40.0M,
                    DateTimeUtc = dateTimeUtcNow.AddMinutes(1),
                },
                new WeatherModel()
                {
                    TemperatureInCelcius = 63.0M,
                    Humidity = 92.0M,
                    DateTimeUtc = dateTimeUtcNow.AddMinutes(2),
                },
            };
            var nextZoneChangeDateTime = InTheZoneService.GetNextZoneChange(weatherModels, thresholds, true);
            Assert.AreEqual(nextZoneChangeDateTime, weatherModels.First().DateTimeUtc);
        }

        [TestCase(false, false, false, true)]
        [TestCase(false, false, true, false)]
        [TestCase(false, true, false, false)]
        [TestCase(false, true, false, true)]
        [TestCase(false, true, true, false)]
        [TestCase(false, true, true, true)]
        [TestCase(true, false, false, false)]
        [TestCase(true, false, false, true)]
        [TestCase(true, false, true, false)]
        [TestCase(true, false, true, true)]
        [TestCase(true, true, false, true)]
        [TestCase(true, true, true, false)]
        public void GetZoneChangeMessageTest(bool lastSensorZone, bool currentSensorZone, bool lastWeatherZone, bool currentWeatherZone)
        {
            var message = InTheZoneService.GenerateZoneChangeMessage(lastSensorZone, currentSensorZone, lastWeatherZone, currentWeatherZone);
            Assert.IsNotEmpty(message);
        }
    }
}