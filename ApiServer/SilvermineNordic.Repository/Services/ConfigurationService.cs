
namespace SilvermineNordic.Repository
{
    public class ConfigurationService : IConfiguration
    {
        private readonly string _storageConnectionString;
        private readonly string _storageName;
        private readonly string _sqlConnectionString;
        public ConfigurationService(string storageConnectionString, string storageName, string sqlConnectionString)
        {
            _storageConnectionString = storageConnectionString;
            _storageName = storageName;
            _sqlConnectionString = sqlConnectionString;
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
    }
}
