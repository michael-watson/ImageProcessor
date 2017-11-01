using System.IO;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace ImageProcessor.Functions
{
	[StorageAccount(EnvironmentVariables.AzureWebJobsStorage)]
	public static class ResizeImage
	{
		[FunctionName(nameof(ResizeImage))]
		public static void Run(
			[BlobTrigger("raw-images/{name}")]Stream myBlob, string name, TraceWriter log,
			[Blob("resize-images/{name}")] out byte[] output)
		{
			output = ImageService.Resize(myBlob);
		}
	}
}