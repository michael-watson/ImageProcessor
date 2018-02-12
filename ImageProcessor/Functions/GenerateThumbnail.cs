using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ProjectOxford.Vision;

namespace ImageProcessor.Functions
{
    public static class GenerateThumbnail
    {
        [FunctionName(nameof(GenerateThumbnail))]
        public static void Run(
            [QueueTrigger(Routes.ThumbnailQueue)]ImageMessage myQueueId, TraceWriter log,
            [Blob(Routes.ThumbnailBlob)] out byte[] output,
            [Queue(Routes.DataSaveQueue)] out ImageMessage contentMessage)
        {
            if (myQueueId is null)
            {
                log.Info($"{myQueueId.Id}: {nameof(GenerateThumbnail)} - {nameof(ImageMessage)} is null");
                output = null;
                contentMessage = null;
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
                }
                else
                {
                    contentMessage = new ImageMessage(myQueueId.Id, MessageType.Thumbnail, $"{Routes.BaseBlobUriRoute}{Routes.ThumbnailQueue}/{myQueueId.Id}");
                    log.Info($"{myQueueId.Id}: Thumbnail created successfully");
                }
            }
            catch (ClientException e)
            {
                log.Info($"{myQueueId.Id}: {nameof(GenerateThumbnail)} Client Error - {e.Message}");
                output = null;
                contentMessage = null;
            }
        }
    }
}
