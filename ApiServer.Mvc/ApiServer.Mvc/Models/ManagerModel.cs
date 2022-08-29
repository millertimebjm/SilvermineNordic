using ApiServer.Model;

namespace ApiServer.Mvc.Models
{
    public enum PostResultEnum
    {
        SaveSuccess,
        SaveFailed,
        DeleteSuccess,
        DeleteFailed,
    }

    public class ManagerModel
    {
        public Task<IEnumerable<Item>> ItemsTask { get; set; }
        public Task<User> UserTask { get; set; }
        public PostResultEnum? PostResult { get; set; }
    }
}
