using System;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ImageAnalysisBlock.Models;

namespace ImageAnalysisBlock.Functions
{
    public static class SendNotification
    {
        static NotificationHubClient hubCLient = NotificationHubClient.CreateClientFromConnectionString(EnvironmentVariables.NotificationHubConnectionString, EnvironmentVariables.NotificationHubNameString);

        [FunctionName(nameof(SendNotification))]
        public static void Run(
            [QueueTrigger(Routes.ImageProcessed)]ImageMessage imageMessage,
            TraceWriter log,
            [Table(nameof(PendingNotification), "{Id}", "Apple")] PendingNotification appleMessageAlreadySent,
            [Table(nameof(PendingNotification))] out PendingNotification pendingNotification)
        {
            if (imageMessage?.Id is null)
            {
                pendingNotification = null;
                log.Error($"{nameof(SendNotification)}: Analyzed data was null for {imageMessage.Id}");
            }
            else if (appleMessageAlreadySent != null)
            {
                pendingNotification = null;
                log.Error($"{nameof(SendNotification)}: Notification was already sent for {imageMessage.Id}");
            }
            else
            {
                try
                {
                    var body = new JProperty("body", "Image Processed");
                    var contentAvail = new JProperty("content-available", 1);
                    var alertPayload = new JProperty("acme2", imageMessage.Id);

                    var alert = new JObject(new JProperty("alert", new JObject(body)), alertPayload);
                    var everything = new JObject(alertPayload, new JProperty("aps", alert)).ToString(Formatting.None);

                    var outcome = hubCLient.SendAppleNativeNotificationAsync(everything, imageMessage.Id)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();

                    pendingNotification = new PendingNotification(imageMessage.Id, outcome);

                    log.Info($"Sent push notification for {imageMessage.Id}");
                }
                catch (JsonException je)
                {
                    pendingNotification = null;
                    log.Error(null, je);
                }
                catch (Exception e)
                {
                    pendingNotification = null;
                    log.Error(null, e);
                }
            }
        }
    }
}