using Microsoft.EntityFrameworkCore;
using SilvermineNordic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilvermineNordic.Repository.Services
{
    public class EntityFrameworkThresholdService : IRepositoryThreshold
    {
        private readonly SilvermineNordicDbContext _dbContext;
        public EntityFrameworkThresholdService(SilvermineNordicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Threshold>> GetThresholds(
            int? count = null,
            int? skip = null)
        {
            return await _dbContext
                .Thresholds.AsQueryable()
                .Skip(skip ?? 0)
                .Take(count ?? int.MaxValue)
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
    }
}
