using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml;
using ImageAnalysisBlock.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace ImageAnalysisBlock.Functions
{
    public static class BuildTelemetryMessages
    {
        [FunctionName(nameof(BuildTelemetryMessages))]
        public static void Run(
            [TimerTrigger("0 */10 * * * *")]TimerInfo myTimer,
            [Table(nameof(PendingNotification))] IQueryable<PendingNotification> tableBinding,
            [Queue(Routes.AnalyzeNotification)] ICollector<ImageMessage> myQueueItems,
            TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

            var allPendingNotifications = tableBinding.ToList();

            //Get list of all IDs that need to be created
            foreach (PendingNotification pendingNotification in allPendingNotifications)
            {
                myQueueItems.Add(new ImageMessage(pendingNotification.NotificationId) { Data = pendingNotification.PartitionKey });
            }
        }
    }
}