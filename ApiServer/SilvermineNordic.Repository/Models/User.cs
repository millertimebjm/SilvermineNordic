namespace SilvermineNordic.Repository.Models
{
    public class User
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? LastLoggedInUtc { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
