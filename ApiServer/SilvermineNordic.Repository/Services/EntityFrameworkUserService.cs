using Microsoft.EntityFrameworkCore;
using SilvermineNordic.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<User?> GetUserAsync(string email)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(_ => _.Email == email);
        }

        public Task<User> UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}
