using ApiServer.Model;

namespace ApiServer.WebApi.Models
{
    public class UserModel
    {
        public Guid? UserId { get; set; }
        public string Username { get; set; }

        public UserModel(User user)
        {
            if (user != null)
            {
                UserId = user.UserId ?? new Guid();
                Username = user.Username;
            }
        }
    }
}
