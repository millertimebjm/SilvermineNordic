using SilvermineNordic.Repository.Models;

namespace SilvermineNordic.Repository.Services
{
    public interface IRepositorySensorReading
    {
        public Task<SensorReading> AddSensorReadingAsync(SensorReading sensorReading);
        public Task<SensorReading> GetLatestSensorReadingAsync();
    }
}
