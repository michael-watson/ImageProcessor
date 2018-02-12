using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json;

namespace ImageProcessor.Functions
{
    [StorageAccount(EnvironmentVariables.AzureWebJobsStorage)]
    public static class DataSaver
    {
        [FunctionName(nameof(DataSaver))]
        public static void Run(
            [QueueTrigger(Routes.DataSaveQueue)]ImageMessage myQueueId, TraceWriter log,
            [DocumentDB(Routes.Metadata, Routes.AnalyzedImages, Id = "{id}")] ProcessedImage analyzedData)
        {
            dynamic data = null;

            try
            {
                switch (myQueueId.Type)
                {
                    case MessageType.GenericAnalysis:
                        data = JsonConvert.DeserializeObject<AnalysisResult>(myQueueId.Data);
                        analyzedData.GenericAnalysis = data;
                        break;
                    case MessageType.OcrAnalysis:
                        data = JsonConvert.DeserializeObject<OcrResults>(myQueueId.Data);
                        analyzedData.OcrData = data;
                        analyzedData.OcrDrawImageUrl = $"{Routes.BaseBlobUriRoute}{Routes.OcrDrawTextQueue}/{myQueueId.Id}";
                        break;
                    case MessageType.Thumbnail:
                        data = myQueueId.Data;
                        analyzedData.ThumbnailUrl = data;
                        break;
                    default:
                        log.Info($"{myQueueId.Id}: unsupported message type - {myQueueId.Type.ToString()}");
                        break;
                }
            }
            catch (JsonException je)
            {
                log.Info($"{myQueueId.Id}: Bad formatted object {nameof(DataSaver)} - {myQueueId.Type.ToString()} - {data.ToString()}");
            }
        }
    }
}