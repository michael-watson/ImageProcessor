using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;

using Newtonsoft.Json;

using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace ImageProcessor
{
	public static class ImageService
	{
		static readonly IVisionServiceClient visionClient = new VisionServiceClient(EnvironmentVariables.ComputerVisionApiKey, EnvironmentVariables.VisionUrl);

		public static string RecognizeText(string imageUrl)
		{
			var ocrData = visionClient.RecognizeTextAsync(imageUrl).GetAwaiter().GetResult();

			return JsonConvert.SerializeObject(ocrData);
		}

		public static byte[] DrawBoxesOnImageText(Image img, OcrResults ocrData)
		{
			var imageWithRectangles = new Bitmap(img.Width, img.Height);
			imageWithRectangles.SetResolution(img.HorizontalResolution, img.VerticalResolution);

			using (var graphics = Graphics.FromImage(imageWithRectangles))
			{
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
			}

			ImageConverter converter = new ImageConverter();
			return (byte[])converter.ConvertTo(imageWithRectangles, typeof(byte[]));
		}

		public static byte[] Resize(Stream imageStream)
		{
			Image image = Image.FromStream(imageStream);

			var imageRect = getNewImageSize(image);
			var destRect = new System.Drawing.Rectangle(0, 0, imageRect.Width, imageRect.Height);
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

		static System.Drawing.Rectangle getNewImageSize(Image image)
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