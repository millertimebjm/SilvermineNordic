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
        private readonly ISilvermineNordicDbContextFactory _dbContextFactory;

        public EntityFrameworkThresholdService(
            SilvermineNordicDbContext dbContext,
            ISilvermineNordicDbContextFactory dbContextFactory)
        {
            _dbContext = dbContext;
            _dbContextFactory = dbContextFactory;
        }

        public async Task<IEnumerable<Threshold>> GetThresholds(
            int? count = null,
            int? skip = null)
        {
            using var context = _dbContextFactory.Create();
            if (context.Database.ProviderName == INMEMORY
                && (await context.Thresholds.FirstOrDefaultAsync()) == null)
            {
                await SeedData();
            }
            var queryable = context.Thresholds.AsQueryable();
            if (skip != null && skip > 0) queryable = queryable.Skip(skip.Value);
            if (count != null && count > 0) queryable = queryable.Take(count.Value);
            return await queryable.ToListAsync();
        }

        public async Task<Threshold> UpsertThreshold(Threshold threshold)
        {
            using var context = _dbContextFactory.Create();
            if (threshold.Id > 0)
            {
                threshold = context.Update(threshold).Entity;
                await context.SaveChangesAsync();
                return threshold;
            }
            await context.AddAsync(threshold);
            await context.SaveChangesAsync();
            return threshold;
        }

        private async Task SeedData()
        {
            using var context = _dbContextFactory.Create();
            await context.Thresholds.AddAsync(new Threshold()
            {
                Id = 0,
                HumidityHighThreshold = 10m,
                HumidityLowThreshold = 0m,
                TemperatureInCelciusHighThreshold = 11m,
                TemperatureInCelciusLowThreshold = 1m,
            });
            await context.SaveChangesAsync();
        }
    }
}
