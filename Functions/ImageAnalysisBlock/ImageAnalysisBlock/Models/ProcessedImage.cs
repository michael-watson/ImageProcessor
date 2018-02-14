using Microsoft.Azure.Documents;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageAnalysisBlock
{
    public class ProcessedImage : Entity
    {
        public ProcessedImage()
        {
        }

        public string Name { get; set; }
        public string OriginalImageUrl { get; set; }
        public string CogsReadyImageUrl { get; set; }
        public string OcrDrawImageUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public OcrResults OcrData { get; set; }
        public AnalysisResult GenericAnalysis { get; set; }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder("\n\nProcessedImage\n");
            sb.Append("  Id".PadRight(20));
            sb.Append($"{id}\n");
            sb.Append("  DocumentId".PadRight(20));
            sb.Append("  OriginalImageUrl".PadRight(20));
            sb.Append($"{OriginalImageUrl}\n");
            sb.Append("  TextBoxImageUrl".PadRight(20));
            sb.Append($"{OcrDrawImageUrl}\n");
            return sb.ToString();
        }
    }
}
