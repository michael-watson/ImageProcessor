using ImageAnalysisBlock.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.ProjectOxford.Vision;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml;

namespace ImageAnalysisBlock.Functions
{
    [StorageAccount(EnvironmentVariables.AzureWebJobsStorage)]

    public static class TryGetTelemetry
    {
        static HttpClient client = new HttpClient();

        [FunctionName(nameof(TryGetTelemetry))]
        public static void Run(
            [QueueTrigger(Routes.AnalyzeNotification)]ImageMessage myQueueId,
            TraceWriter log,
            [Table(nameof(NotificationResult))] out NotificationResult pendingNotification)
        {
            if (myQueueId is null)
            {
                pendingNotification = null;
                log.Error("Pending Notification was null");
            }
            if (myQueueId.Id is null)
            {
                pendingNotification = null;
                log.Error("Pending NotificationID was null");
            }
            if (myQueueId.Data is null)
            {
                pendingNotification = null;
                log.Error("Pending Notification tag was null");
            }

            string hubResource = "messages/" + myQueueId.Id + "?";
            string apiVersion = "api-version=2016-07";
            ConnectionStringUtility connectionSasUtil = new ConnectionStringUtility(EnvironmentVariables.NotificationHubConnectionString);

            //=== Generate SaS Security Token for Authentication header ===
            // Determine the targetUri that we will sign
            string uri = connectionSasUtil.Endpoint + "image-analyzed-notificator/" + hubResource + apiVersion;
            string sasToken = connectionSasUtil.getSaSToken(uri, 60);

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            HttpWebResponse response = null;

            request.Method = "GET";

            if (sasToken != null)
                request.Headers.Add("Authorization", sasToken);

            request.Headers.Add("x-ms-version", "2016-07");

            try
            {
                response = (HttpWebResponse)request.GetResponseAsync().Result;
                using (Stream receiveStream = response.GetResponseStream())
                // Pipes the stream to a higher level stream reader with the required encoding format. 
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    var xml = readStream.ReadToEndAsync().Result;
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    string json = JsonConvert.SerializeXmlNode(doc);
                    var notificaitonDetails = JsonConvert.DeserializeObject<RootNotificationObject>(json);

                    if (notificaitonDetails?.NotificationDetails is null)
                        pendingNotification = null;
                    else
                    {
                        pendingNotification = new NotificationResult(myQueueId.Data, notificaitonDetails.NotificationDetails);
                        var result = CloudTableHelper.Delete(pendingNotification.Tags, pendingNotification.NotificationId).GetAwaiter().GetResult();
                        log.Info($"Successfully got telemetry data for message{pendingNotification.NotificationId}");
                    }
                }
            }
            catch (WebException we)
            {
                pendingNotification = null;

                if (we.Response != null)
                {
                    response = (HttpWebResponse)we.Response;
                }
                else
                    Console.WriteLine(we.Message);
            }
            catch (Exception e)
            {
                pendingNotification = null;

                Console.WriteLine(e.Message);
            }
        }
    }
}
