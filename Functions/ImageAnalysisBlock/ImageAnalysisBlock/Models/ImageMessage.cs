using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace ImageAnalysisBlock
{
    public enum MessageType
    {
        None = 0,
        GenericAnalysis = 1,
        OcrAnalysis = 2,
        Thumbnail = 3
    }

    [JsonObject]
    public class ImageMessage
    {
        [JsonConstructor]
        public ImageMessage() { }
        public ImageMessage(string id)
        {
            Id = id;
        }
        public ImageMessage(string id, MessageType type, string data)
        {
            Id = id;
            Type = type;
            Data = data;
        }

        [JsonProperty("id")]
        public string Id { get; set; }
        public MessageType Type { get; set; } = MessageType.None;
        public string Data { get; set; }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder("\n\nProcessedImage\n");
            sb.Append("  Id".PadRight(20));
            sb.Append($"{Id}\n");
            return sb.ToString();
        }
    }
}
