
namespace SilvermineNordic.Repository
{
    public interface IConfiguration
    {
        public string GetStorageConnectionString();
        public string GetStorageName();
        public string GetSqlConnectionString();
        public string GetOpenWeatherApiKey();
        public string GetAzureSmsConnectionString();
        public string GetAzureSmsFromPhone();
    }
}