
namespace SilvermineNordic.Repository
{
    public class ConfigurationService : IConfiguration
    {
        private readonly string _storageConnectionString;
        private readonly string _storageName;
        private readonly string _sqlConnectionString;
        private readonly string _openWeatherApiKey;
        public ConfigurationService(
            string storageConnectionString, 
            string storageName, 
            string sqlConnectionString, 
            string openWeatherApiKey)
        {
            _storageConnectionString = storageConnectionString;
            _storageName = storageName;
            _sqlConnectionString = sqlConnectionString;
            _openWeatherApiKey = openWeatherApiKey;
        }

        public string GetSqlConnectionString()
        {
            return _sqlConnectionString;
        }

        public string GetStorageConnectionString()
        {
            return _storageConnectionString;
        }

        public string GetStorageName()
        {
            return _storageName;
        }

        public string GetOpenWeatherApiKey()
        {
            return _openWeatherApiKey;
        }
    }
}
