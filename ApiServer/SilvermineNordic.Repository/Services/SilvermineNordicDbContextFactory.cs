using Microsoft.EntityFrameworkCore;

namespace SilvermineNordic.Repository.Services
{
    public enum DbContextTypeEnum
    {
        InMemory = 0,
        SqlServer = 1,
    }
    public class SilvermineNordicDbContextFactory : ISilvermineNordicDbContextFactory
    {
        public readonly string _connectionString;
        public readonly DbContextTypeEnum _type;
        public SilvermineNordicDbContextFactory(string connectionString, DbContextTypeEnum type)
        {
            _connectionString = connectionString;
            _type = type;
        }

        public SilvermineNordicDbContext GetDbContext()
        {
            DbContextOptionsBuilder<SilvermineNordicDbContext> builder = new DbContextOptionsBuilder<SilvermineNordicDbContext>();
            switch (_type)
            {
                case DbContextTypeEnum.InMemory:
                    builder.UseInMemoryDatabase(_connectionString);
                    return new SilvermineNordicDbContext(builder.Options);
                case DbContextTypeEnum.SqlServer:
                    builder.UseSqlServer(_connectionString);
                    return new SilvermineNordicDbContext(builder.Options);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
