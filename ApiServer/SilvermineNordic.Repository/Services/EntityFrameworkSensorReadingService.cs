
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
            return await _dbContext.SensorReadings
                .Where(_ => _.Type == SensorReadingTypeEnum.Sensor.ToString())
                .OrderByDescending(_ => _.Id)
                .FirstAsync();
        }

        public async Task<SensorReading> GetLatestWeatherReadingAsync()
        {
            return await _dbContext.SensorReadings
                .Where(_ => _.Type == SensorReadingTypeEnum.Weather.ToString())
                .OrderByDescending(_ => _.Id)
                .FirstAsync();
        }

        public async Task<IEnumerable<SensorReading>> GetLastTwoWeatherReadingAsync()
        {
            return await _dbContext.SensorReadings
                .Where(_ => _.Type == SensorReadingTypeEnum.Weather.ToString())
                .OrderByDescending(_ => _.Id)
                .Take(2)
                .ToListAsync();
        }

        public async Task<IEnumerable<SensorReading>> GetLastTwoSensorReadingAsync()
        {
            return await _dbContext.SensorReadings
                .Where(_ => _.Type == SensorReadingTypeEnum.Sensor.ToString())
                .OrderByDescending(_ => _.Id)
                .Take(2)
                .ToListAsync();
        }
    }
}
