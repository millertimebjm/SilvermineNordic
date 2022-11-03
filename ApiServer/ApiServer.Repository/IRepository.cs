using ApiServer.Model;

namespace ApiServer.Repository
{
    public interface IRepository
    {
        Task<string?> GetItemValueByKeyAsync(Guid key);
        Task<Item> SetItemAsync(Item item);
        Task<User?> GetUserAsync(Guid userId);
        Task<User> SetUserAsync(User user);
        Task<User?> GetUserAsync(string username, string password);
        Task<IEnumerable<Item>> GetItemsAsync(Guid userId);
        Task<bool> GetUserExists(string username);
    }
}