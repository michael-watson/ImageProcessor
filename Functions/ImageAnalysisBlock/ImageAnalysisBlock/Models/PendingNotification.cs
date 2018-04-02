using Microsoft.Azure.NotificationHubs;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageAnalysisBlock.Models
{
    public class PendingNotification : TableEntity
    {
        public PendingNotification()
        {
        }

        public PendingNotification(string uniqueTag, NotificationOutcome outcome)
        {
            PartitionKey = uniqueTag;
            RowKey = outcome.NotificationId;
            NotificationId = outcome.NotificationId;
            TrackingId = outcome.TrackingId;
        }
        public string NotificationId { get; set; }
        public string TrackingId { get; set; }
    }
}
