
namespace SilvermineNordic.Repository.Services
{
    public interface ISilvermineNordicDbContextFactory
    {
        public SilvermineNordicDbContext GetDbContext();
    }
}
