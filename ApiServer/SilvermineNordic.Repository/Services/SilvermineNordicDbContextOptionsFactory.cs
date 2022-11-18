using Microsoft.EntityFrameworkCore;

namespace SilvermineNordic.Repository.Services
{
    public enum DbContextTypeEnum
    {
        InMemory = 0,
        SqlServer = 1,
    }
    public class SilvermineNordicDbContextOptionsFactory : ISilvermineNordicDbContextOptionsFactory
    {
        public readonly string _connectionString;
        public readonly DbContextTypeEnum _type;
        public SilvermineNordicDbContextOptionsFactory(string connectionString, DbContextTypeEnum type)
        {
            _connectionString = connectionString;
            _type = type;
        }

        public DbContextOptions<SilvermineNordicDbContext> GetDbContextOptions()
        {
            var builder = new DbContextOptionsBuilder<SilvermineNordicDbContext>();
            switch (_type)
            {
                case DbContextTypeEnum.InMemory:
                    builder.UseInMemoryDatabase(_connectionString);
                    return builder.Options;
                case DbContextTypeEnum.SqlServer:
                    builder.UseSqlServer(_connectionString);
                    return builder.Options;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
