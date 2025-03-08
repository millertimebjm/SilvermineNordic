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
        private readonly SilvermineNordicDbContext _dbContext;

        public EntityFrameworkThresholdService(
            SilvermineNordicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Threshold>> GetThresholds(
            int? count = null,
            int? skip = null)
        {
            var queryable = _dbContext.Thresholds.AsQueryable();
            if (skip != null && skip > 0) queryable = queryable.Skip(skip.Value);
            if (count != null && count > 0) queryable = queryable.Take(count.Value);
            return await queryable.ToListAsync();
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
    }
}
