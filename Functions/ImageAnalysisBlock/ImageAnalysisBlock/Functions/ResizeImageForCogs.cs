using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using ImageProcessor;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace ImageAnalysisBlock.Functions
{
    [StorageAccount(EnvironmentVariables.AzureWebJobsStorage)]
    public static class ResizeImageForCogs
    {
        [FunctionName(nameof(ResizeImageForCogs))]
        public static void Run(
            [BlobTrigger(Routes.OriginalImageBlob)]Stream myBlob, string id, TraceWriter log,
            [DocumentDB(Routes.Metadata, Routes.AnalyzedImages)] out ProcessedImage analyzedData,
            [Blob(Routes.CogsImageBlob)] out byte[] output)
        {
            log.Info($"{id}: Processing image");

            Image image = Image.FromStream(myBlob);
            Rectangle destRect = new Rectangle();

            log.Info($"{id}: Image Width - {image.Width}");
            log.Info($"{id}: Image Height - {image.Height}");

            if (image.Height > 2000 || image.Width > 2000)
            {
                log.Info($"{id}: Image will be resized");

                var imageRect = ImageService.GetRectForCogsImage(image);
                destRect = new Rectangle(0, 0, imageRect.Width, imageRect.Height);


                log.Info($"{id}: Resized Width: {imageRect.Width}");
                log.Info($"{id}: Resized Height: {imageRect.Height}");
            }
            else
            {
                log.Info($"{id}: No resizing needed");

                destRect = new Rectangle(0, 0, image.Width, image.Height);
            }

            using (var processor = new ImageFactory())
            using (var stream = new MemoryStream())
            {
                image.ExifRotate();
                processor.Load(image);
                processor.PreserveExifData = true;
                processor.Quality(50);
                processor.AutoRotate();
                processor.Resize(new Size(destRect.Width, destRect.Height)).Save(stream);
                output = stream.ToArray();
            }

            analyzedData = new ProcessedImage
            {
                id = id,
                OriginalImageUrl = $"{Routes.BaseBlobUriRoute}{Routes.OriginalImageBlob.Replace("{id}", id)}",
                CogsReadyImageUrl = $"{Routes.BaseBlobUriRoute}{Routes.CogsImageBlob.Replace("{id}", id)}"
            };

            //using (var destImage = new Bitmap(imageRect.Width, imageRect.Height))
            //using (var stream = new MemoryStream())
            //using (var graphics = Graphics.FromImage(destImage))
            //{
            //    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            //    graphics.CompositingMode = CompositingMode.SourceCopy;
            //    graphics.CompositingQuality = CompositingQuality.HighQuality;
            //    graphics.InterpolationMode = InterpolationMode.Low;
            //    graphics.SmoothingMode = SmoothingMode.HighQuality;
            //    graphics.PixelOffsetMode = PixelOffsetMode.Half;
            //    graphics.DrawImage(image, destRect, imageRect.X, imageRect.Y, imageRect.Width, imageRect.Height, GraphicsUnit.Pixel);

            //    destImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            //    output = stream.ToArray();
            //}

            //analyzedData = new ProcessedImage
            //{
            //    id = id,
            //    OriginalImageUrl = $"{Routes.BaseBlobUriRoute}{Routes.OriginalImageBlob.Replace("{id}", id)}",
            //    CogsReadyImageUrl = $"{Routes.BaseBlobUriRoute}{Routes.CogsImageBlob.Replace("{id}", id)}"
            //};
        }
        private const int exifOrientationID = 0x112; //274

        public static void ExifRotate(this Image img)
        {
            if (!img.PropertyIdList.Contains(exifOrientationID))
                return;

            var prop = img.GetPropertyItem(exifOrientationID);
            int val = BitConverter.ToUInt16(prop.Value, 0);
            var rot = RotateFlipType.RotateNoneFlipNone;

            if (val == 3 || val == 4)
                rot = RotateFlipType.Rotate180FlipNone;
            else if (val == 5 || val == 6)
                rot = RotateFlipType.Rotate90FlipNone;
            else if (val == 7 || val == 8)
                rot = RotateFlipType.Rotate270FlipNone;

            if (val == 2 || val == 4 || val == 5 || val == 7)
                rot |= RotateFlipType.RotateNoneFlipX;

            if (rot != RotateFlipType.RotateNoneFlipNone)
                img.RotateFlip(rot);
        }

        public static RotateFlipType GetRotateFlipTypeByExifOrientationData(int orientation)
        {
            switch (orientation)
            {
                case 1:
                default:
                    return RotateFlipType.RotateNoneFlipNone;
                case 2:
                    return RotateFlipType.RotateNoneFlipX;
                case 3:
                    return RotateFlipType.Rotate180FlipNone;
                case 4:
                    return RotateFlipType.Rotate180FlipX;
                case 5:
                    return RotateFlipType.Rotate90FlipX;
                case 6:
                    return RotateFlipType.Rotate90FlipNone;
                case 7:
                    return RotateFlipType.Rotate270FlipX;
                case 8:
                    return RotateFlipType.Rotate270FlipNone;
            }
        }
    }
}