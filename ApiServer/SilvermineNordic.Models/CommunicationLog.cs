namespace SilvermineNordic.Models
{
    public enum CommunicationTypeEnum
    {
        Email = 0,
        SMS = 1,
    }
    public class CommunicationLog
    {
        public int Id { get; set; }
        public CommunicationTypeEnum Type { get; set; }
        public int CommunicationTypeId
        {
            get => (int)Type;
            set => Type = (CommunicationTypeEnum)value;
        }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Destination { get; set; }
        public DateTime DateTimestampUtc { get; set; } = DateTime.UtcNow;
        public bool Success { get; set; } = false;
    }
}
