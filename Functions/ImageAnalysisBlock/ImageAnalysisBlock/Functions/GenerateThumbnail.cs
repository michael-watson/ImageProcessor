using System;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ProjectOxford.Vision;

namespace ImageAnalysisBlock.Functions
{
    public static class GenerateThumbnail
    {
        [FunctionName(nameof(GenerateThumbnail))]
        public static void Run(
            [QueueTrigger(Routes.ThumbnailQueue)]ImageMessage myQueueId, TraceWriter log,
            [Blob(Routes.ThumbnailBlob)] out byte[] output,
            [Queue(Routes.DataSaveQueue)] out ImageMessage contentMessage,
            [Queue(Routes.ImageProcessed)] out ImageMessage processedImage)
        {
            if (myQueueId is null || myQueueId?.Data is null)
            {
                log.Info($"{myQueueId.Id}: {nameof(GenerateThumbnail)} - {nameof(ImageMessage)} is null");
                output = null;
                contentMessage = null;
                processedImage = null;
                return;
            }

            log.Info($"{myQueueId.Id}: Creating thumbnail");

            try
            {
                output = ImageService.GetThumbnail(myQueueId.Data);

                if (output is null)
                {
                    log.Info($"{myQueueId.Id}: Unable to generate thumbnail");
                    contentMessage = null;
                    output = null;
                    processedImage = null;
                }
                else
                {
                    contentMessage = new ImageMessage(myQueueId.Id, MessageType.Thumbnail, $"{Routes.BaseBlobUriRoute}{Routes.ThumbnailQueue}/{myQueueId.Id}");
                    processedImage = new ImageMessage(myQueueId.Id);
                    log.Info($"{myQueueId.Id}: Thumbnail created successfully");
                }
            }
            catch (ClientException e)
            {
                log.Info($"{myQueueId.Id}: {nameof(GenerateThumbnail)} Client Error - {e.Message}");

                output = null;
                processedImage = null;
                contentMessage = null;
            }
        }
    }
}