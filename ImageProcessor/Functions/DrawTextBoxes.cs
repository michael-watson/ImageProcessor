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
            if(myQueueId == null || originalEntry == null)
            {
                log.Info("Nothing to process: DrawTextBoxes");
                output = null;
                return;
            }

            log.Info($"C# Queue trigger function processed: {myQueueId}");

            Image img = Image.FromStream(resizedImage);

            var imageWithRectangles = new Bitmap(img.Width, img.Height);
            imageWithRectangles.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            using (var graphics = Graphics.FromImage(imageWithRectangles))
            {
                var ocrData = JsonConvert.DeserializeObject<OcrResults>(originalEntry?.OcrData);

                graphics.DrawImage(img, new System.Drawing.Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);
                foreach (var region in ocrData?.Regions)
                {
                    foreach(var line in region.Lines)
                    {
                        var bounds = line?.BoundingBox?.Split(',') ?? null;

                        if (bounds != null)
                        {
                            var boxToDraw = new System.Drawing.Rectangle(int.Parse(bounds[0]), int.Parse(bounds[1]), int.Parse(bounds[2]), int.Parse(bounds[3]));

                            graphics.DrawRectangle(new Pen(System.Drawing.Color.SpringGreen, 2), boxToDraw);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Unable to draw: {bounds.Count()}");
                        }
                    }
                }
            }

            originalEntry.TextBoxImageUrl = $"https://image-processing-miwats.documents.azure.com:443/text-boxes/{myQueueId.Id}";

            ImageConverter converter = new ImageConverter();
            output = (byte[])converter.ConvertTo(imageWithRectangles, typeof(byte[]));
        }
    }
}