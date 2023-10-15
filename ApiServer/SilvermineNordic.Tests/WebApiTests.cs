using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Moq;
using SilvermineNordic.Models;
using SilvermineNordic.Repository;
using SilvermineNordic.Repository.Services;

namespace SilvermineNordic.Tests
{
    public class WebApiTests
    {
        private IRepositoryReading _readingService;
        private SilvermineNordicDbContext _silvermineNordicDbContext;


        public WebApiTests()
        {
            var iOptionsSnapshotMock = new Mock<IOptionsSnapshot<SilvermineNordicConfigurationService>>();
            _silvermineNordicDbContext = new SilvermineNordicDbContext(iOptionsSnapshotMock.Object);
            _readingService = new EntityFrameworkReadingService(_silvermineNordicDbContext);

            _silvermineNordicDbContext.Readings.AddRange(new List<Reading>()
                {
                    new Reading()
                    {
                        //Id = 0,
                        DateTimestampUtc= DateTime.UtcNow.AddMinutes(-11),
                        InsertedDateTimestampUtc= DateTime.UtcNow.AddMinutes(-11),
                        ReadingDateTimestampUtc = DateTime.UtcNow.AddMinutes(-11),
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = ReadingTypeEnum.Weather.ToString(),
                    },
                    new Reading()
                    {
                        //Id = 1,
                        DateTimestampUtc= DateTime.UtcNow.AddMinutes(-10),
                        InsertedDateTimestampUtc= DateTime.UtcNow.AddMinutes(-10),
                        ReadingDateTimestampUtc = DateTime.UtcNow.AddMinutes(-10),
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = ReadingTypeEnum.Sensor.ToString(),
                    },new Reading()
                    {
                        //Id = 2,
                        DateTimestampUtc= DateTime.UtcNow.AddMinutes(-6),
                        InsertedDateTimestampUtc= DateTime.UtcNow.AddMinutes(-6),
                        ReadingDateTimestampUtc = DateTime.UtcNow.AddMinutes(-6),
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = ReadingTypeEnum.Weather.ToString(),
                    },
                    new Reading()
                    {
                        //Id = 3,
                        DateTimestampUtc= DateTime.UtcNow.AddMinutes(-5),
                        InsertedDateTimestampUtc= DateTime.UtcNow.AddMinutes(-5),
                        ReadingDateTimestampUtc = DateTime.UtcNow.AddMinutes(-5),
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = ReadingTypeEnum.Sensor.ToString(),
                    },
                    new Reading()
                    {
                        //Id = 4,
                        DateTimestampUtc= DateTime.UtcNow.AddMinutes(-1),
                        InsertedDateTimestampUtc= DateTime.UtcNow.AddMinutes(-1),
                        ReadingDateTimestampUtc = DateTime.UtcNow.AddMinutes(-1),
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = ReadingTypeEnum.Weather.ToString(),
                    },new Reading()
                    {
                        //Id = 5,
                        DateTimestampUtc= DateTime.UtcNow,
                        InsertedDateTimestampUtc= DateTime.UtcNow,
                        ReadingDateTimestampUtc = DateTime.UtcNow,
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = ReadingTypeEnum.Sensor.ToString(),
                    },
                });
            _silvermineNordicDbContext.SaveChanges();
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
            var readings = await _readingService.GetLastNReadingAsync(ReadingTypeEnum.Sensor, 1);
            Assert.AreEqual(readings.Count(), 1);
        }

        [Test]
        public async Task GetLatestNReadingsTest2()
        {
            var readings = await _readingService.GetLastNReadingAsync(ReadingTypeEnum.Weather, 1);
            Assert.AreEqual(readings.Count(), 1);
        }

        [Test]
        public async Task GetLatestNReadingsTest3()
        {
            var readings = await _readingService.GetLastNReadingAsync(ReadingTypeEnum.Sensor, 2);
            Assert.AreEqual(readings.Count(), 2);
        }

        [Test]
        public async Task GetLatestNReadingsTest4()
        {
            var readings = await _readingService.GetLastNReadingAsync(ReadingTypeEnum.Weather, 2);
            Assert.AreEqual(readings.Count(), 2);
        }
    }
}
