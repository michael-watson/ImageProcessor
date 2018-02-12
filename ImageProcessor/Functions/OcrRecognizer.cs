using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.ProjectOxford.Vision;
using Newtonsoft.Json;

namespace ImageProcessor.Functions
{
    [StorageAccount(EnvironmentVariables.AzureWebJobsStorage)]
    public static class OcrRecognizer
    {
        [FunctionName(nameof(OcrRecognizer))]
        public static void Run(
            [QueueTrigger(Routes.OcrAnalysisQueue)]ImageMessage myQueueId, TraceWriter log,
            [Queue(Routes.OcrDrawTextQueue)] out ImageMessage contentMessage)
        {
            if (myQueueId is null)
            {
                log.Info($"{myQueueId.Id}: {nameof(OcrRecognizer)} - {nameof(ProcessedImage.CogsReadyImageUrl)} is null");
                contentMessage = null;
                return;
            }

            log.Info($"{myQueueId.Id}: Processing for Ocr");

            try
            {
                var ocrData = ImageService.RecognizeText(myQueueId.Data);

                if (ocrData is null)
                {
                    log.Info($"{myQueueId.Id}: No text found");
                    contentMessage = null;
                }
                else
                {
                    var ocrModel = JsonConvert.SerializeObject(ocrData);
                    contentMessage = new ImageMessage(myQueueId.Id, MessageType.OcrAnalysis, ocrModel);
                    log.Info($"{myQueueId.Id}: {nameof(ProcessedImage.CogsReadyImageUrl)} processed successfully");
                }
            }
            catch (ClientException e)
            {
                log.Info($"{myQueueId.Id}: {nameof(OcrRecognizer)} Client Error - {e.Message}");
                contentMessage = null;
            }
        }
    }
}