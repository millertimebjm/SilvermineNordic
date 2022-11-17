using SilvermineNordic.Repository.Models;

namespace SilvermineNordic.Repository.Services
{
    public interface IRepositoryUser
    {
        public Task AddUserAsync(User user);
        public Task UpdateUserAsync(User user);
        public Task DeleteUserAsync(User user);
        public Task<User> GetUserAsync(string email);
    }
}
