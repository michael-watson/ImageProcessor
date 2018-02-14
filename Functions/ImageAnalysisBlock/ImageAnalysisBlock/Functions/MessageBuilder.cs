using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ImageAnalysisBlock
{
    [StorageAccount(EnvironmentVariables.AzureWebJobsStorage)]
    public static class MessageBuilder
    {
        static CloudStorageAccount StorageAccount = CloudStorageAccount.Parse(EnvironmentVariables.StorageAccountConnection);

        [FunctionName(nameof(MessageBuilder))]
        public static void Run(
            [BlobTrigger(Routes.CogsImageBlob)] CloudBlockBlob inputBlob, string id, TraceWriter log,
            [DocumentDB(Routes.Metadata, Routes.AnalyzedImages, Id = "{id}")] ProcessedImage analyzedData,
            [Queue(Routes.GenericAnalysisQueue)] out ImageMessage genericAnalysisMessage,
            [Queue(Routes.OcrAnalysisQueue)] out ImageMessage ocrAnalysisMessage,
            [Queue(Routes.ThumbnailQueue)] out ImageMessage thumbnail)
        {
            if (analyzedData is null || string.IsNullOrEmpty(analyzedData.CogsReadyImageUrl))
            {
                log.Info($"{id}: {nameof(ProcessedImage.CogsReadyImageUrl)} not populated");
                genericAnalysisMessage = null;
                ocrAnalysisMessage = null;
                thumbnail = null;
                return;
            }

            log.Info($"{id}: Image ready to process - {inputBlob.Uri.AbsoluteUri}");

            genericAnalysisMessage = new ImageMessage(id) { Data = analyzedData.CogsReadyImageUrl };
            ocrAnalysisMessage = new ImageMessage(id) { Data = analyzedData.CogsReadyImageUrl };
            thumbnail = new ImageMessage(id) { Data = analyzedData.CogsReadyImageUrl };
        }
    }
}