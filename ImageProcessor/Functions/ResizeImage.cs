using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Drawing;
using System;
using System.Drawing.Drawing2D;

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
            output = Resize(myBlob);
        }

        public static byte[] Resize(Stream imageStream)
        {
            Image image = Image.FromStream(imageStream);

            var imageRect = getNewImageSize(image);
            var destRect = new Rectangle(0, 0, imageRect.Width, imageRect.Height);
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

            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(destImage, typeof(byte[]));
        }

        static Rectangle getNewImageSize(Image image)
        {
            if (image.Height < 2000 && image.Width < 2000)
                return new Rectangle(0, 0, image.Width, image.Height);

            var originalWidth = image.Width;
            var originalHeight = image.Height;

            double ratio = (double)image.Height / (double)image.Width;

            double canvasWidth = 2000 * ratio;
            double canvasHeight = 2000;

            // Figure out the ratio
            double ratioX = (double)canvasWidth / (double)originalWidth;
            double ratioY = (double)canvasHeight / (double)originalHeight;
            // use whichever multiplier is smaller
            ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
            int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

            return new Rectangle(posX, posY, newWidth, newHeight);
        }
    }
}
