using SilvermineNordic.Repository.Models;

namespace SilvermineNordic.Repository.Services
{
    public interface IRepositorySensorReading
    {
        public Task<SensorReading> AddSensorReadingAsync(SensorReading sensorReading);
        public Task<IEnumerable<SensorReading>> GetLastNReadingAsync(SensorReadingTypeEnum type, int count);
    }
}

