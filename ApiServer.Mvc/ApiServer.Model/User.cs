using System.ComponentModel.DataAnnotations;

namespace ApiServer.Model
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Item> Items { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj as User != null)
            {
                return Equals((User)obj);
            }
            return base.Equals(obj);
        }

        public bool Equals(User user)
        {
            return user.UserId == UserId
                || (user.Username == Username && Password == Password);
        }
    }
}