using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ImageProcessor.Functions
{
    [StorageAccount(EnvironmentVariables.AzureWebJobsStorage)]
    public static class TextRecognizer
    {
        private static readonly IVisionServiceClient visionClient = new VisionServiceClient(EnvironmentVariables.ComputerVisionApiKey, EnvironmentVariables.VisionUrl);

        [FunctionName(nameof(TextRecognizer))]
        public static void Run(
            [QueueTrigger("image-to-process")]ImageMessage myQueueId, TraceWriter log,
            [DocumentDB("metadata", "analyzed-images", Id = "{Id}")] ProcessedImage originalEntry,
            [Queue("ocr-draw")] out ImageMessage contentMessage)
        {
            if(originalEntry == null)
            {
                log.Info("Nothing to process: TextRecognizer");
                contentMessage = null;
                return;
            }

            log.Info($"C# Queue trigger function processed: {myQueueId.Id}");

            try
            {
                var ocrData = visionClient.RecognizeTextAsync(originalEntry.OriginalImageUrl, "unk", true).GetAwaiter().GetResult();
                originalEntry.OcrData = JsonConvert.SerializeObject(ocrData);

                contentMessage = new ImageMessage(originalEntry.Id);
            }
            catch (ClientException e)
            {
                log.Info($"Unable to process: {e.Message}");
                contentMessage = null;
            }
        }
    }
}
