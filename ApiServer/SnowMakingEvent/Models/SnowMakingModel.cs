using Azure;
using Azure.Data.Tables;
using System;

namespace SnowMakingEvent.Models
{
    public class SnowMakingModel : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTime Timestamp { get; set; }
        public string LastHumidity { get; set; }
        public string LastTemperatureInCelcius { get; set; }
        public DateTime LastReading { get; set; }
        private ETag _eTag;
        public ETag ETag { get => _eTag; set => _eTag = value; }
        private DateTimeOffset? _timestamp;
        DateTimeOffset? ITableEntity.Timestamp { get => _timestamp; set => _timestamp = value; }
    }
}
