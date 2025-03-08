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

        public async Task<IEnumerable<Reading>> GetLastNReadingAsync(
            ReadingTypeEnum type,
            int count,
            int? skip = 0)
        {
            IQueryable<Reading> queryable = _dbContext
                .Readings.Where(_ => _.Type == type.ToString())
                .OrderByDescending(_ => _.Id);
            if (skip != null && skip > 0) queryable = queryable.Skip(skip.Value);
            return await queryable
                .Take(count)
                .ToListAsync();
        }
    }
}
