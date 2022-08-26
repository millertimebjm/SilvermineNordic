using ApiServer.Model;

namespace ApiServer.Mvc.Models
{
    public class ManagerModel
    {
        public Task<IEnumerable<Item>> ItemsTask { get; set; }
        public Task<User> UserTask { get; set; }
        public bool? PostResult { get; set; }
    }
}
