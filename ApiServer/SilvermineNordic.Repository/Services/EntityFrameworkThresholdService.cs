using Microsoft.EntityFrameworkCore;
using SilvermineNordic.Repository.Models;

namespace SilvermineNordic.Repository.Services
{
    public class EntityFrameworkThresholdService : IRepositoryThreshold
    {
        private readonly SilvermineNordicDbContext _dbContext;
        public EntityFrameworkThresholdService(SilvermineNordicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Threshold> GetThreshold()
        {
            return await _dbContext.Thresholds.SingleAsync();
        }

        public async Task<Threshold> UpdateThreshold(Threshold threshold)
        {
            _dbContext.Update(threshold);
            await _dbContext.SaveChangesAsync();
            return threshold;
        }
    }
}
