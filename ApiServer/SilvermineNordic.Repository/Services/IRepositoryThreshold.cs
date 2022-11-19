using SilvermineNordic.Repository.Models;

namespace SilvermineNordic.Repository.Services
{
    public interface IRepositoryThreshold
    {
        public Task<Threshold> UpdateThreshold(Threshold threshold);
        public Task<IEnumerable<Threshold>> GetThresholds();
    }
}
