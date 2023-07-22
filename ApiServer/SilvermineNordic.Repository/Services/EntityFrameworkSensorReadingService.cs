using Microsoft.EntityFrameworkCore;
using SilvermineNordic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilvermineNordic.Repository.Services
{
    public class EntityFrameworkReadingService : IRepositoryReading
    {
        private readonly SilvermineNordicDbContext _dbContext;
        private const string INMEMORY = "Microsoft.EntityFrameworkCore.InMemory";
        public EntityFrameworkReadingService(
            SilvermineNordicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Reading> AddReadingAsync(Reading reading)
        {
            await _dbContext.Readings.AddAsync(reading);
            await _dbContext.SaveChangesAsync();
            return reading;
        }

        public async Task<IEnumerable<Reading>> GetLastNReadingAsync(ReadingTypeEnum type, int count)
        {
            if (_dbContext.Database.ProviderName == INMEMORY
                && (await _dbContext.Readings.FirstOrDefaultAsync()) == null)
            {
                await SeedData();
            }

            return await _dbContext.Readings
                .Where(_ => _.Type == type.ToString())
                .OrderByDescending(_ => _.Id)
                .Take(count)
                .ToListAsync();
        }

        private async Task SeedData()
        {
            await _dbContext.Readings.AddAsync(new Reading()
            {
                DateTimestampUtc = DateTime.UtcNow,
                Humidity = 20,
                InsertedDateTimestampUtc = DateTime.UtcNow,
                ReadingDateTimestampUtc = DateTime.UtcNow,
                TemperatureInCelcius = 30,
                Type = ReadingTypeEnum.Sensor.ToString(),
            });
            await _dbContext.Readings.AddAsync(new Reading()
            {
                DateTimestampUtc = DateTime.UtcNow,
                Humidity = 21,
                InsertedDateTimestampUtc = DateTime.UtcNow,
                ReadingDateTimestampUtc = DateTime.UtcNow,
                TemperatureInCelcius = 31,
                Type = ReadingTypeEnum.Weather.ToString(),
            });
            await _dbContext.SaveChangesAsync();
        }
    }
}
