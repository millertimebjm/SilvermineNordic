using Microsoft.EntityFrameworkCore;
using SilvermineNordic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilvermineNordic.Repository.Services
{
    public class EntityFrameworkSensorReadingService : IRepositorySensorReading
    {
        private readonly SilvermineNordicDbContext _dbContext;
        private const string INMEMORY = "Microsoft.EntityFrameworkCore.InMemory";
        public EntityFrameworkSensorReadingService(
            SilvermineNordicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SensorReading> AddSensorReadingAsync(SensorReading sensorReading)
        {
            await _dbContext.SensorReadings.AddAsync(sensorReading);
            await _dbContext.SaveChangesAsync();
            return sensorReading;
        }

        public async Task<IEnumerable<SensorReading>> GetLastNReadingAsync(SensorReadingTypeEnum type, int count)
        {
            if (_dbContext.Database.ProviderName == INMEMORY
                && (await _dbContext.SensorReadings.FirstOrDefaultAsync()) == null)
            {
                await SeedData();
            }

            return await _dbContext.SensorReadings
                .Where(_ => _.Type == type.ToString())
                .OrderByDescending(_ => _.Id)
                .Take(count)
                .ToListAsync();
        }

        private async Task SeedData()
        {
            await _dbContext.SensorReadings.AddAsync(new SensorReading()
            {
                DateTimestampUtc = DateTime.Now,
                Humidity = 20,
                InsertedDateTimestampUtc = DateTime.Now,
                ReadingDateTimestampUtc = DateTime.Now,
                TemperatureInCelcius = 30,
                Type = SensorReadingTypeEnum.Sensor.ToString(),
            });
            await _dbContext.SensorReadings.AddAsync(new SensorReading()
            {
                DateTimestampUtc = DateTime.Now,
                Humidity = 21,
                InsertedDateTimestampUtc = DateTime.Now,
                ReadingDateTimestampUtc = DateTime.Now,
                TemperatureInCelcius = 31,
                Type = SensorReadingTypeEnum.Weather.ToString(),
            });
            await _dbContext.SaveChangesAsync();
        }
    }
}
