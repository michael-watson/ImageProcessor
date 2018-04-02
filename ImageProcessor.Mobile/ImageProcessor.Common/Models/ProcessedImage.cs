using Newtonsoft.Json;

namespace ImageProcessor.Common.Models
{
    public class ProcessedImage
    {
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }
        public string Name { get; set; }
        public string OriginalImageUrl { get; set; }
        public string CogsReadyImageUrl { get; set; }
        public string OcrDrawImageUrl { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}