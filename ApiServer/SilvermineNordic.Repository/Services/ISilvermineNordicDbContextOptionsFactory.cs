using Microsoft.EntityFrameworkCore;

namespace SilvermineNordic.Repository.Services
{
    public interface ISilvermineNordicDbContextOptionsFactory
    {
        public DbContextOptions<SilvermineNordicDbContext> GetDbContextOptions();
    }
}
