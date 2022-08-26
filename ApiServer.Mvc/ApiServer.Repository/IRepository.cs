using ApiServer.Model;

namespace ApiServer.Repository
{
    public interface IRepository
    {
        Item GetItemAsync(Guid userId, Guid itemId, string identifier);
        Item SetItemAsync(Item item);
        Task<User> GetUserAsync(Guid userId);
        User SetUserAsync(User user);
        User GetUserAsync(string username, string password);
        Task<IEnumerable<Item>> GetItemsAsync(Guid userId);
    }
}