using System;
using System.Linq;
using System.Drawing;

using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using ImageAnalysisBlock.Extensions;

namespace ImageAnalysisBlock
{
    public static class ImageService
    {
        static readonly IVisionServiceClient visionClient = new VisionServiceClient(EnvironmentVariables.ComputerVisionApiKey, EnvironmentVariables.VisionUrl);

        public static OcrResults RecognizeText(string imageUrl) =>
            visionClient.RecognizeTextAsync(imageUrl)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
        public static AnalysisResult GenericAnalysis(string imageUrl) =>
            visionClient.AnalyzeImageAsync(imageUrl, null)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
        public static byte[] GetThumbnail(string imageUrl) =>
            visionClient.GetThumbnailAsync(imageUrl, 57, 57)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();

        public static byte[] DrawBoxesOnImageText(Image img, OcrResults ocrData)
        {
            var codec = ImageCodecInfo.GetImageDecoders().FirstOrDefault(x => x.FormatID == ImageFormat.Png.Guid);
            var myEncoder = Encoder.Quality;
            var myEncoderParameters = new EncoderParameters(1);

            var myEncoderParameter = new EncoderParameter(myEncoder, 50L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            using (var imageWithRectangles = new Bitmap(img.Width, img.Height))
            using (var imageStream = new MemoryStream())
            using (var graphics = Graphics.FromImage(imageWithRectangles))
            {
                imageWithRectangles.SetResolution(img.HorizontalResolution, img.VerticalResolution);

                graphics.InterpolationMode = InterpolationMode.Low;
                graphics.DrawImage(img, new System.Drawing.Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel);

                foreach (var region in ocrData?.Regions)
                {
                    foreach (var line in region.Lines)
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

                imageWithRectangles.Save(imageStream, codec, myEncoderParameters);
                return imageStream.ReadToEnd();
            }


            //ImageConverter converter = new ImageConverter();
            //return (byte[])converter.ConvertTo(imageWithRectangles, typeof(byte[]));
        }

        public static System.Drawing.Rectangle GetRectForCogsImage(Image image)
        {
            if (image.Height < 2000 && image.Width < 2000)
                return new System.Drawing.Rectangle(0, 0, image.Width, image.Height);

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

            return new System.Drawing.Rectangle(posX, posY, newWidth, newHeight);
        }
    }
}