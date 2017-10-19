namespace ImageProcessor.Mobile
{
	public class ProcessedImage
	{
		public ProcessedImage()
		{
		}
		public string Id { get; set; }
		public string Name { get; set; }
		public string OriginalImageUrl { get; set; }
		public string TextBoxImageUrl { get; set; }
		public string TextRecognizedImageUrl { get; set; }
		public string TextRecognizedThumbnailUrl { get; set; }
		public string OcrData { get; set; }
	}
}