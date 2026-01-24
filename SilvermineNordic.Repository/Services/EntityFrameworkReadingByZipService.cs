using Microsoft.EntityFrameworkCore;
using SilvermineNordic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilvermineNordic.Repository.Services
{
    public class EntityFrameworkReadingByZipService : IReadingByZip
    {
        private readonly SilvermineNordicDbContext _dbContext;
        
        public EntityFrameworkReadingByZipService(
            SilvermineNordicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<string>> GetForRefresh()
        {
            var cutoff = DateTime.UtcNow.AddDays(-1);
            return await _dbContext
                .ReadingByZips
                .Where(rbz => rbz.LastLookupUtc > cutoff)
                .Select(rbz => rbz.Zip)
                .ToListAsync();
        }

        public async Task<ReadingByZip?> Get(string zip)
        {
            var models = await _dbContext.ReadingByZips.SingleOrDefaultAsync(rbz => rbz.Zip == zip);
            return models;
        }

        public async Task Upsert(string zip, string weatherDataSerialized)
        {
            var readingByZip = await _dbContext.ReadingByZips.SingleOrDefaultAsync(rbz => rbz.Zip == zip);
            if (readingByZip is null)
            {
                readingByZip = new ReadingByZip()
                {
                    Zip = zip,
                    WeatherDataSerialized = weatherDataSerialized
                };
                await _dbContext.AddAsync(readingByZip);
            }
            else
            {
                readingByZip.WeatherDataSerialized = weatherDataSerialized;
                readingByZip.LastUpdatedUtc = DateTime.UtcNow;
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
