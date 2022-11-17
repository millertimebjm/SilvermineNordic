
using Microsoft.EntityFrameworkCore;
using SilvermineNordic.Repository.Models;

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

        public async Task<SensorReading> GetLatestSensorReadingAsync()
        {
            return await _dbContext.SensorReadings.OrderByDescending(_ => _.Id).FirstAsync();
        }
    }
}
