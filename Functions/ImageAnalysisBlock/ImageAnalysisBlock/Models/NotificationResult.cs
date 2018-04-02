using System;
using Microsoft.Azure.NotificationHubs;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageAnalysisBlock.Models
{
    public enum NotificationPlatofrm
    {
        Apple,
        Google
    }
    public class NotificationResult
    {
        public NotificationResult(string partitionKey, NotificationDetails outcome)
        {
            PartitionKey = partitionKey;
            RowKey = outcome.NotificationId;
            NotificationId = outcome.NotificationId;

            State = outcome.State;
            Location = outcome.Location;
            StartTime = outcome.StartTime;
            EndTime = outcome.EndTime;
            NotificationBody = outcome.NotificationBody;
            Tags = outcome.Tags;
            ApnsOutcomeCounts = outcome.ApnsOutcomeCounts;

            var minDateTime = new DateTime(2000, 1, 1);

            if (StartTime < minDateTime)
                StartTime = minDateTime;
            if (EndTime < minDateTime)
                EndTime = minDateTime;
            if (EnqueueTime < minDateTime)
                EnqueueTime = minDateTime;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public string TrackingId { get; set; }
        public string NotificationId { get; set; }
        public string State { get; set; }

        [JsonProperty("Location")]
        public string Location { get; set; }
        [JsonProperty("EnqueueTime")]
        public System.DateTimeOffset EnqueueTime { get; set; }
        [JsonProperty("StartTime")]
        public System.DateTimeOffset StartTime { get; set; }
        [JsonProperty("EndTime")]
        public System.DateTimeOffset EndTime { get; set; }
        [JsonProperty("NotificationBody")]
        public string NotificationBody { get; set; }
        [JsonProperty("Tags")]
        public string Tags { get; set; }
        [JsonProperty("ApnsOutcomeCounts")]
        public ApnsOutcomeCounts ApnsOutcomeCounts { get; set; }
    }
}
