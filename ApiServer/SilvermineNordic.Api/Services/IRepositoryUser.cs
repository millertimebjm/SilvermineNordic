using SilvermineNordic.Api.Models;

namespace SilvermineNordic.Api.Services
{
    public interface IRepositoryUser
    {
        public Task AddUserAsync(User user);
        public Task UpdateUserAsync(User user);
        public Task DeleteUserAsync(User user);
        public Task<User> GetUser(string email);
    }
}
