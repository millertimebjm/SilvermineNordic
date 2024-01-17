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
        //private SilvermineNordicDbContext _silvermineNordicDbContext;
        private readonly ISilvermineNordicDbContextFactory _silvermineNordicDbContextFactory;

        public WebApiTests()
        {
            var iOptionsSnapshotMock = new Mock<IOptionsSnapshot<SilvermineNordicConfigurationService>>();
            _silvermineNordicDbContextFactory = new SilvermineNordicDbContextFactory(iOptionsSnapshotMock.Object);
            _readingService = new EntityFrameworkReadingService(_silvermineNordicDbContextFactory);

            var silvermineNordicDbContext = _silvermineNordicDbContextFactory.Create();
            silvermineNordicDbContext.Readings.AddRange(new List<Reading>()
                {
                    new Reading()
                    {
                        //Id = 0,
                        DateTimeUtc= DateTime.UtcNow.AddMinutes(-11),
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = ReadingTypeEnum.Weather.ToString(),
                    },
                    new Reading()
                    {
                        //Id = 1,
                        DateTimeUtc= DateTime.UtcNow.AddMinutes(-10),
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = ReadingTypeEnum.Sensor.ToString(),
                    },new Reading()
                    {
                        //Id = 2,
                        DateTimeUtc= DateTime.UtcNow.AddMinutes(-6),
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = ReadingTypeEnum.Weather.ToString(),
                    },
                    new Reading()
                    {
                        //Id = 3,
                        DateTimeUtc= DateTime.UtcNow.AddMinutes(-5),
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = ReadingTypeEnum.Sensor.ToString(),
                    },
                    new Reading()
                    {
                        //Id = 4,
                        DateTimeUtc= DateTime.UtcNow.AddMinutes(-1),
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = ReadingTypeEnum.Weather.ToString(),
                    },new Reading()
                    {
                        //Id = 5,
                        DateTimeUtc= DateTime.UtcNow,
                        TemperatureInCelcius = 30m,
                        Humidity = 50m,
                        Type = ReadingTypeEnum.Sensor.ToString(),
                    },
                });
            silvermineNordicDbContext.SaveChanges();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            //_silvermineNordicDbContext.Dispose();
        }

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task GetLatestNReadingsTest1()
        {
            var readings = await _readingService.GetLastNReadingAsync(ReadingTypeEnum.Sensor, 1);
            Assert.That(readings.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetLatestNReadingsTest2()
        {
            var readings = await _readingService.GetLastNReadingAsync(ReadingTypeEnum.Weather, 1);
            Assert.That(readings.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetLatestNReadingsTest3()
        {
            var readings = await _readingService.GetLastNReadingAsync(ReadingTypeEnum.Sensor, 2);
            Assert.That(readings.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetLatestNReadingsTest4()
        {
            var readings = await _readingService.GetLastNReadingAsync(ReadingTypeEnum.Weather, 2);
            Assert.That(readings.Count(), Is.EqualTo(2));
        }
    }
}
