using SilvermineNordic.Models;
using System.Threading.Tasks;

namespace SilvermineNordic.Repository.Services
{
    public interface IRepositoryUser
    {
        public Task<User> AddUserAsync(User user);
        public Task<User> UpdateUserAsync(User user);
        public Task DeleteUserAsync(User user);
        public Task<User?> GetUserAsync(string email);
    }
}
