using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using SilvermineNordic.Models;
using SilvermineNordic.Repository;
using SilvermineNordic.Repository.Services;

namespace SilvermineNordic.Tests
{
    public class WebApiTests
    {
        private ISilvermineNordicConfiguration _configuration;
        private IRepositorySensorReading _sensorReadingService;
        private SilvermineNordicDbContext _silvermineNordicDbContext;


        public WebApiTests()
        {
            _configuration = new SilvermineNordicConfigurationService()
            {
                InMemoryDatabaseName = "test",
            };
            _silvermineNordicDbContext = new SilvermineNordicDbContext(_configuration);
            _sensorReadingService = new EntityFrameworkSensorReadingService(_silvermineNordicDbContext);

            _silvermineNordicDbContext.SensorReadings.AddRange(new List<SensorReading>()
                {
                    new SensorReading()
                    {
                        //Id = 0,
                        DateTimestampUtc= DateTime.UtcNow.AddMinutes(-11),
                        InsertedDateTimestampUtc= DateTime.UtcNow.AddMinutes(-11),
                        ReadingDateTimestampUtc = DateTime.UtcNow.AddMinutes(-11),
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = SensorReadingTypeEnum.Weather.ToString(),
                    },
                    new SensorReading()
                    {
                        //Id = 1,
                        DateTimestampUtc= DateTime.UtcNow.AddMinutes(-10),
                        InsertedDateTimestampUtc= DateTime.UtcNow.AddMinutes(-10),
                        ReadingDateTimestampUtc = DateTime.UtcNow.AddMinutes(-10),
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = SensorReadingTypeEnum.Sensor.ToString(),
                    },new SensorReading()
                    {
                        //Id = 2,
                        DateTimestampUtc= DateTime.UtcNow.AddMinutes(-6),
                        InsertedDateTimestampUtc= DateTime.UtcNow.AddMinutes(-6),
                        ReadingDateTimestampUtc = DateTime.UtcNow.AddMinutes(-6),
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = SensorReadingTypeEnum.Weather.ToString(),
                    },
                    new SensorReading()
                    {
                        //Id = 3,
                        DateTimestampUtc= DateTime.UtcNow.AddMinutes(-5),
                        InsertedDateTimestampUtc= DateTime.UtcNow.AddMinutes(-5),
                        ReadingDateTimestampUtc = DateTime.UtcNow.AddMinutes(-5),
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = SensorReadingTypeEnum.Sensor.ToString(),
                    },
                    new SensorReading()
                    {
                        //Id = 4,
                        DateTimestampUtc= DateTime.UtcNow.AddMinutes(-1),
                        InsertedDateTimestampUtc= DateTime.UtcNow.AddMinutes(-1),
                        ReadingDateTimestampUtc = DateTime.UtcNow.AddMinutes(-1),
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = SensorReadingTypeEnum.Weather.ToString(),
                    },new SensorReading()
                    {
                        //Id = 5,
                        DateTimestampUtc= DateTime.UtcNow,
                        InsertedDateTimestampUtc= DateTime.UtcNow,
                        ReadingDateTimestampUtc = DateTime.UtcNow,
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = SensorReadingTypeEnum.Sensor.ToString(),
                    },
                });
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _silvermineNordicDbContext.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public async Task GetLatestNReadingsTest1()
        {
            var readings = await _sensorReadingService.GetLastNReadingAsync(SensorReadingTypeEnum.Sensor, 1);
        }

        [Test]
        public async Task GetLatestNReadingsTest2()
        {
            var readings = await _sensorReadingService.GetLastNReadingAsync(SensorReadingTypeEnum.Weather, 1);
        }

        [Test]
        public async Task GetLatestNReadingsTest3()
        {
            var readings = await _sensorReadingService.GetLastNReadingAsync(SensorReadingTypeEnum.Weather, 1);
        }

        [Test]
        public async Task GetLatestNReadingsTest4()
        {
            var readings = await _sensorReadingService.GetLastNReadingAsync(SensorReadingTypeEnum.Weather, 1);
        }
    }
}
