using SilvermineNordic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilvermineNordic.Repository.Services
{
    public interface IRepositoryThreshold
    {
        public Task<Threshold> UpsertThreshold(Threshold threshold);
        public Task<IEnumerable<Threshold>> GetThresholds(int? count = 0, int? skip = 0);

    }
}
