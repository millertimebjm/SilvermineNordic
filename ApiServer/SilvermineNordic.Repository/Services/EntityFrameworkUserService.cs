using Microsoft.EntityFrameworkCore;
using SilvermineNordic.Models;

namespace SilvermineNordic.Repository.Services
{
    public class EntityFrameworkUserService : IRepositoryUser
    {
        private readonly SilvermineNordicDbContext _dbContext;
        public EntityFrameworkUserService(SilvermineNordicDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<User> AddUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserAsync(string email)
        {
            return _dbContext.Users.SingleOrDefault(_ => _.Email == email);
        }

        public Task<User> UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
