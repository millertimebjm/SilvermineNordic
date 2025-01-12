using SilvermineNordic.Repository.Services;
using SilvermineNordic.Repository;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SilvermineNordic.Models;

namespace SilvermineNordic.Tests.RepositoryTests 
{
    public class RepositoryTests
    {
        public SilvermineNordicDbContextFactory GetContextFactory(string inMemoryDatabaseName) 
        {
            SilvermineNordicConfigurationService config = new SilvermineNordicConfigurationService()
            {
                InMemoryDatabaseName = inMemoryDatabaseName, 
            };
            var mock = new Mock<IOptionsSnapshot<SilvermineNordicConfigurationService>>();
            mock.Setup(m => m.Value).Returns(config);
            return new SilvermineNordicDbContextFactory(mock.Object);
        }

        [Test]
        public async Task ReadingAdd()
        {
            var dbFactory = GetContextFactory("ReadingAdd");
            IRepositoryReading readingRepositoryService = new EntityFrameworkReadingService(dbFactory);
            var reading = new Reading() 
            {
                Id = 0,
                Type = "Sensor",
                DateTimeUtc = DateTime.UtcNow,
                TemperatureInCelcius = 11.1m,
                Humidity = 21.1m, 
            };
            var newReading = await readingRepositoryService.AddReadingAsync(reading);
            Assert.That(reading.Type, Is.EqualTo(newReading.Type));
            Assert.That(reading.DateTimeUtc, Is.EqualTo(newReading.DateTimeUtc));
            Assert.That(reading.TemperatureInCelcius, Is.EqualTo(newReading.TemperatureInCelcius));
            Assert.That(reading.Humidity, Is.EqualTo(newReading.Humidity));
        }

        [Test]
        public async Task ReadingGet()
        {
            var dbFactory = GetContextFactory("ReadingGet");
            IRepositoryReading readingRepositoryService = new EntityFrameworkReadingService(dbFactory);
            var reading = new Reading()
            {
                Id = 0,
                Type = "Sensor",
                DateTimeUtc = DateTime.UtcNow,
                TemperatureInCelcius = 11.1m,
                Humidity = 21.1m,
            };
            var newReading = await readingRepositoryService.AddReadingAsync(reading);
            var getReading = (await readingRepositoryService.GetLastNReadingAsync(ReadingTypeEnum.Sensor, 1)).First();
            Assert.That(getReading.Type, Is.EqualTo(newReading.Type));
            Assert.That(getReading.DateTimeUtc, Is.EqualTo(newReading.DateTimeUtc));
            Assert.That(getReading.TemperatureInCelcius, Is.EqualTo(newReading.TemperatureInCelcius));
            Assert.That(getReading.Humidity, Is.EqualTo(newReading.Humidity));
        }
    }
}