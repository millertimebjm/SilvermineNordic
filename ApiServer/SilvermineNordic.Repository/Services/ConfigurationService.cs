
namespace SilvermineNordic.Repository
{
    public class ConfigurationService : IConfiguration
    {
        public string StorageConnectionString { get; set; }
        public string StorageName { get; set; }
        public string SqlConnectionString { get; set; }
        public string OpenWeatherApiKey { get; set; }
        public string AzureSmsConnectionString { get; set; }
        public string AzureSmsFromPhone { get; set; }

        public ConfigurationService()
        {
            
        }

        public string GetSqlConnectionString()
        {
            return SqlConnectionString;
        }

        public string GetStorageConnectionString()
        {
            return StorageConnectionString;
        }

        public string GetStorageName()
        {
            return StorageName;
        }

        public string GetOpenWeatherApiKey()
        {
            return OpenWeatherApiKey;
        }

        public string GetAzureSmsConnectionString()
        {
            return AzureSmsConnectionString;
        }

        public string GetAzureSmsFromPhone()
        {
            return AzureSmsFromPhone;
        }
    }
}
