using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.ProjectOxford.Vision.Contract;

namespace ImageProcessor.Functions
{
	[StorageAccount(EnvironmentVariables.AzureWebJobsStorage)]
	public static class DrawTextBoxes
	{
		[FunctionName(nameof(DrawTextBoxes))]
		public static void Run(
			[QueueTrigger("ocr-draw")]ImageMessage myQueueId, TraceWriter log,
			[DocumentDB("metadata", "analyzed-images", Id = "{Id}", CreateIfNotExists = false)] ProcessedImage originalEntry,
			[Blob("resize-images/{Id}", FileAccess.Read)] Stream resizedImage,
			[Blob("text-boxes/{Id}")] out byte[] output)
		{
			if (myQueueId == null || originalEntry == null)
			{
				log.Info("Nothing to process: DrawTextBoxes");
				output = null;
				return;
			}

			log.Info($"C# Queue trigger function processed: {myQueueId}");

			Image img = Image.FromStream(resizedImage);

			var ocrData = JsonConvert.DeserializeObject<OcrResults>(originalEntry?.OcrData);
			output = ImageService.DrawBoxesOnImageText(img, ocrData);
		}
	}
}