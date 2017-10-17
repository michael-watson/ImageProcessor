using System;

using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace ImageProcessor
{
    [StorageAccount(EnvironmentVariables.AzureWebJobsStorage)]
    public static class MessageBuilder
    {
        static CloudStorageAccount StorageAccount = CloudStorageAccount.Parse(EnvironmentVariables.StorageAccountConnection);

        [FunctionName(nameof(MessageBuilder))]
        public static void Run(
            [BlobTrigger("resize-images/{name}")] CloudBlockBlob inputBlob, string name, TraceWriter log,
            [Queue("image-to-process")] out ImageMessage contentMessage,
            [DocumentDB("metadata", "analyzed-images")] out ProcessedImage analyzedData)
        {
            log.Info($"Image ready to process: {inputBlob.Uri.AbsoluteUri}");

            var encodedName = Uri.EscapeDataString(name);

            analyzedData = new ProcessedImage
            {
                Id = name,
                OriginalImageUrl = $"{inputBlob.Container.StorageUri.PrimaryUri.ToString()}/{encodedName}"
            };

            contentMessage = new ImageMessage(analyzedData.Id);
        }
    }
}