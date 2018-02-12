using System.IO;
using System.Drawing;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.ProjectOxford.Vision.Contract;

namespace ImageProcessor.Functions
{
    [StorageAccount(EnvironmentVariables.AzureWebJobsStorage)]
    public static class OcrDraw
    {
        [FunctionName(nameof(OcrDraw))]
        public static void Run(
            [QueueTrigger(Routes.OcrDrawTextQueue)]ImageMessage myQueueId, TraceWriter log,
            [Blob(Routes.CogsImageBlob, FileAccess.Read)] Stream resizedImage,
            [Blob(Routes.OcrDrawImageBlob)] out byte[] output,
            [Queue(Routes.DataSaveQueue)] out ImageMessage contentMessage)
        {
            if (myQueueId is null)
            {
                log.Info($"{myQueueId.Id}: {nameof(OcrDraw)} - {nameof(ImageMessage)} was null");
                output = null;
                contentMessage = null;
                return;
            }

            log.Info($"{myQueueId.Id}: Drawing OCR boxes");

            try
            {
                Image img = Image.FromStream(resizedImage);
                var ocrData = JsonConvert.DeserializeObject<OcrResults>(myQueueId.Data);

                output = ImageService.DrawBoxesOnImageText(img, ocrData);
                contentMessage = new ImageMessage(myQueueId.Id, MessageType.OcrAnalysis, myQueueId.Data);
            }
            catch (JsonException je)
            {
                log.Info($"{myQueueId.Id}: Bad formatted object {nameof(OcrDraw)} - {myQueueId.Type.ToString()} - {myQueueId.Data}");
                output = null;
                contentMessage = null;
            }
        }
    }
}