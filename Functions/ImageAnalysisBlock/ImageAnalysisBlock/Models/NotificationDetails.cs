using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageAnalysisBlock.Models
{
    public class RootNotificationObject
    {
        public NotificationDetails NotificationDetails { get; set; }
    }
    public partial class ApnsOutcomeCounts
    {
        [JsonProperty("Outcome")]
        public Outcome Outcome { get; set; }
    }
    public partial class Outcome
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Count")]
        public string Count { get; set; }
    }

    public class NotificationDetails
    {
        [JsonProperty("@xmlns")]
        public string Xmlns { get; set; }

        [JsonProperty("@xmlns:i")]
        public string XmlnsI { get; set; }

        [JsonProperty("NotificationId")]
        public string NotificationId { get; set; }

        [JsonProperty("Location")]
        public string Location { get; set; }

        [JsonProperty("State")]
        public string State { get; set; }

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

        [JsonProperty("TargetPlatforms")]
        public string TargetPlatforms { get; set; }

        [JsonProperty("ApnsOutcomeCounts")]
        public ApnsOutcomeCounts ApnsOutcomeCounts { get; set; }
    }
}
