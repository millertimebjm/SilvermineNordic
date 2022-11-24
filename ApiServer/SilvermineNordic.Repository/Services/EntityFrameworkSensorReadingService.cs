using Microsoft.EntityFrameworkCore;
using SilvermineNordic.Models;

namespace SilvermineNordic.Repository.Services
{
    public class EntityFrameworkSensorReadingService : IRepositorySensorReading
    {
        private readonly SilvermineNordicDbContext _dbContext;
        public EntityFrameworkSensorReadingService(SilvermineNordicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SensorReading> AddSensorReadingAsync(SensorReading sensorReading)
        {
            await _dbContext.SensorReadings.AddAsync(sensorReading);
            await _dbContext.SaveChangesAsync();
            return sensorReading;
        }

        public async Task<IEnumerable<SensorReading>> GetLastNReadingAsync(SensorReadingTypeEnum type, int count)
        {
            return await _dbContext.SensorReadings
                .Where(_ => _.Type == type.ToString())
                .OrderByDescending(_ => _.Id)
                .Take(count)
                .ToListAsync();
        }
    }
}
