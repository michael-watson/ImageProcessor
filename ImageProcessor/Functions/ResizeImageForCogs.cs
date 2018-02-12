using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace ImageProcessor.Functions
{
    [StorageAccount(EnvironmentVariables.AzureWebJobsStorage)]
    public static class ResizeImageForCogs
    {
        static DocumentClient client = new DocumentClient(new Uri(EnvironmentVariables.DocumentDbUrl), EnvironmentVariables.DocumentDbKey);

        [FunctionName(nameof(ResizeImageForCogs))]
        public static void Run(
            [BlobTrigger(Routes.OriginalImageBlob)]Stream myBlob, string id, TraceWriter log,
            [DocumentDB(Routes.Metadata, Routes.AnalyzedImages)] out ProcessedImage analyzedData,
            [Blob(Routes.CogsImageBlob)] out byte[] output)
        {
            log.Info($"{id}: Processing image");

            Image image = Image.FromStream(myBlob);
            Rectangle imageRect = new Rectangle();
            Rectangle destRect = new Rectangle();

            log.Info($"{id}: Image Width - {image.Width}");
            log.Info($"{id}: Image Height - {image.Height}");

            if (image.Height < 2000 && image.Width < 2000)
            {
                log.Info($"{id}: Image will be resized");

                imageRect = ImageService.GetRectForCogsImage(image);
                destRect = new Rectangle(0, 0, imageRect.Width, imageRect.Height);

                log.Info($"{id}: Resized Width: {imageRect.Width}");
                log.Info($"{id}: Resized Height: {imageRect.Height}");
            }
            {
                log.Info($"{id}: No resizing needed");

                imageRect = new Rectangle(0, 0, image.Width, image.Height);
                destRect = new Rectangle(0, 0, imageRect.Width, imageRect.Height);
            }

            var destImage = new Bitmap(imageRect.Width, imageRect.Height);
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(image, destRect, imageRect.X, imageRect.Y, imageRect.Width, imageRect.Height, GraphicsUnit.Pixel);
            }

            analyzedData = new ProcessedImage
            {
                id = id,
                OriginalImageUrl = $"{Routes.BaseBlobUriRoute}/{Routes.OriginalImageBlob.Replace("{id}", id)}",
                CogsReadyImageUrl = $"{Routes.BaseBlobUriRoute}/{Routes.CogsImageBlob.Replace("{id}", id)}"
            };

            ImageConverter converter = new ImageConverter();
            output = (byte[])converter.ConvertTo(destImage, typeof(byte[]));
        }
    }
}