using Microsoft.Azure.Documents;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor
{
    public class ProcessedImage 
    {
        public ProcessedImage()
        {
        }

        [JsonProperty("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string OriginalImageUrl { get; set; }
        public string TextBoxImageUrl { get; set; }
        public string TextRecognizedImageUrl { get; set; }
        public string TextRecognizedThumbnailUrl { get; set; }
        public string OcrData { get; set; }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder("\n\nProcessedImage\n");
            sb.Append("  Id".PadRight(20));
            sb.Append($"{Id}\n");
            sb.Append("  DocumentId".PadRight(20));
            sb.Append("  OriginalImageUrl".PadRight(20));
            sb.Append($"{OriginalImageUrl}\n");
            sb.Append("  TextBoxImageUrl".PadRight(20));
            sb.Append($"{TextBoxImageUrl}\n");
            return sb.ToString();
        }
    }
}
