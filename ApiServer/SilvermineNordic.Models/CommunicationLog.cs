using System;

namespace SilvermineNordic.Models
{
    public enum CommunicationTypeEnum
    {
        Email = 0,
        SMS = 1,
    }
    public class CommunicationLog
    {
        public int Id { get; set; } = 0;
        public CommunicationTypeEnum Type { get; set; } = CommunicationTypeEnum.Email;
        public int CommunicationTypeId
        {
            get => (int)Type;
            set => Type = (CommunicationTypeEnum)value;
        }
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DateTimestampUtc { get; set; } = DateTime.UtcNow;
        public bool Success { get; set; } = false;
    }
}
