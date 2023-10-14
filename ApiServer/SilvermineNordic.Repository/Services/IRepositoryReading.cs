using SilvermineNordic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SilvermineNordic.Repository.Services
{
    public interface IRepositoryReading
    {
        public Task<Reading> AddReadingAsync(Reading sensorReading);
        public Task<IEnumerable<Reading>> GetLastNReadingAsync(ReadingTypeEnum type, int count, int? skip = 0);
    }
}

