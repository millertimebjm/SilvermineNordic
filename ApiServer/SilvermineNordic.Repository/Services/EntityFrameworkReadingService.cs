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
        private readonly ISilvermineNordicDbContextFactory _dbContextFactory;

        private const string INMEMORY = "Microsoft.EntityFrameworkCore.InMemory";
        public EntityFrameworkReadingService(
            ISilvermineNordicDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<Reading> AddReadingAsync(Reading reading)
        {
            var context = _dbContextFactory.Create();
            await context.Readings.AddAsync(reading);
            await context.SaveChangesAsync();
            return reading;
        }

        public async Task<IEnumerable<Reading>> GetLastNReadingAsync(
            ReadingTypeEnum type,
            int count,
            int? skip = 0)
        {
            using var context = _dbContextFactory.Create();
            if (context.Database.ProviderName == INMEMORY
                && (await context.Readings.FirstOrDefaultAsync()) == null)
            {
                await SeedData();
            }

            IQueryable<Reading> queryable = context
                .Readings.Where(_ => _.Type == type.ToString())
                .OrderByDescending(_ => _.Id);
            if (skip != null && skip > 0) queryable = queryable.Skip(skip.Value);
            return await queryable
                .Take(count)
                .ToListAsync();
        }

        private async Task SeedData()
        {
            using var context = _dbContextFactory.Create();
            await context.Readings.AddAsync(new Reading()
            {
                DateTimeUtc = DateTime.UtcNow,
                Humidity = 20,
                TemperatureInCelcius = 30,
                Type = ReadingTypeEnum.Sensor.ToString(),
            });
            await context.Readings.AddAsync(new Reading()
            {
                DateTimeUtc = DateTime.UtcNow,
                Humidity = 21,
                TemperatureInCelcius = 31,
                Type = ReadingTypeEnum.Weather.ToString(),
            });
            await context.SaveChangesAsync();
        }
    }
}
