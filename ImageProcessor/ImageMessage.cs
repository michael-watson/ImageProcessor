using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace ImageProcessor
{
    [JsonObject]
    public class ImageMessage
    {
        [JsonConstructor]
        public ImageMessage() { }
        public ImageMessage(string id)
        {
            Id = id;
        }
        [JsonProperty("id")]
        public string Id { get; set; }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder("\n\nProcessedImage\n");
            sb.Append("  Id".PadRight(20));
            sb.Append($"{Id}\n");
            return sb.ToString();
        }
    }
}
