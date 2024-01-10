using System;

namespace SilvermineNordic.Models
{
    public class User
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime? LastLoggedInUtc { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
