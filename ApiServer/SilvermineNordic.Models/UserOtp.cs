namespace SilvermineNordic.Models
{
    public class UserOtp
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Guid Otp { get; set; }
        public Guid AuthKey { get; set; }
        public bool Exhausted { get; set; } = false;
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime? LastUsedUtc { get; set; }
    }
}
