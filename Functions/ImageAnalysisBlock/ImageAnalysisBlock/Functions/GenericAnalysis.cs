using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ProjectOxford.Vision;
using Newtonsoft.Json;

namespace ImageAnalysisBlock.Functions
{
    public static class GenericAnalysis
    {
        [FunctionName(nameof(GenericAnalysis))]
        public static void Run(
            [QueueTrigger(Routes.GenericAnalysisQueue)]ImageMessage myQueueId, TraceWriter log,
            [Queue(Routes.DataSaveQueue)] out ImageMessage contentMessage)
        {
            if (myQueueId is null)
            {
                log.Info($"{myQueueId.Id}: {nameof(GenericAnalysis)} - {nameof(ImageMessage)} is null");
                contentMessage = null;
                return;
            }

            log.Info($"{myQueueId.Id}: Processing for Generic Vision Api");

            try
            {
                var genericAnalysis = ImageService.GenericAnalysis(myQueueId.Data);

                if (genericAnalysis is null)
                {
                    contentMessage = null;
                    log.Info($"{myQueueId.Id}: Generic Analysis returned null");
                }
                else
                {
                    var content = JsonConvert.SerializeObject(genericAnalysis);
                    contentMessage = new ImageMessage(myQueueId.Id, MessageType.GenericAnalysis, content);
                    log.Info($"{myQueueId.Id}: Generic Analysis complete");
                }
            }
            catch (ClientException e)
            {
                log.Info($"{myQueueId.Id}: {nameof(GenericAnalysis)} Client Error - {e.Message}");
                contentMessage = null;
            }
        }
    }
}