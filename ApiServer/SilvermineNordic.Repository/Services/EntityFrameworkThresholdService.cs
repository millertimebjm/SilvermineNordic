using Microsoft.EntityFrameworkCore;
using SilvermineNordic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilvermineNordic.Repository.Services
{
    public class EntityFrameworkThresholdService : IRepositoryThreshold
    {
        private const string INMEMORY = "Microsoft.EntityFrameworkCore.InMemory";
        private readonly SilvermineNordicDbContext _dbContext;
        public EntityFrameworkThresholdService(SilvermineNordicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Threshold>> GetThresholds(
            int? count = null,
            int? skip = null)
        {
            if (_dbContext.Database.ProviderName == INMEMORY
                && (await _dbContext.Thresholds.FirstOrDefaultAsync()) == null)
            {
                await SeedData();
            }

            return await _dbContext
                .Thresholds
                .AsQueryable()
                .Skip(skip ?? 0)
                .Take(count ?? 100)
                .ToListAsync();
        }

        public async Task<Threshold> UpsertThreshold(Threshold threshold)
        {
            if (threshold.Id > 0)
            {
                threshold = _dbContext.Update(threshold).Entity;
                await _dbContext.SaveChangesAsync();
                return threshold;
            }
            await _dbContext.AddAsync(threshold);
            await _dbContext.SaveChangesAsync();
            return threshold;
        }

        private async Task SeedData()
        {
            await _dbContext.Thresholds.AddAsync(new Threshold()
            {
                Id = 0,
                HumidityHighThreshold = 10m,
                HumidityLowThreshold = 0m,
                TemperatureInCelciusHighThreshold = 11m,
                TemperatureInCelciusLowThreshold = 1m,
            });
            await _dbContext.SaveChangesAsync();
        }
    }
}
