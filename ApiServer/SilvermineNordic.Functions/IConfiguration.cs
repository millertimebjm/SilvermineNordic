
namespace SilvermineNordic.Functions
{
    public interface IConfiguration
    {
        public string GetStorageConnectionString();
        public string GetStorageName();
        public string GetSqlConnectionString();
    }
}