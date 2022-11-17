using Microsoft.EntityFrameworkCore;
using SilvermineNordic.Repository.Models;

namespace SilvermineNordic.Repository.Services
{
    public class EntityFrameworkUserService : IRepositoryUser
    {
        DbContextOptions<SilvermineNordicDbContext> _options;
        public EntityFrameworkUserService(DbContextOptions<SilvermineNordicDbContext> options)
        {
            _options = options;
        }


        public Task AddUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserAsync(string email)
        {
            using (var db = new SilvermineNordicDbContext(_options))
            {
                return await db.Users.SingleOrDefaultAsync(_ => _.Email == email);
            }
        }

        public Task UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
