using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ImageProcessor
{
    public class Entity
    {
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty("_etag")]
        public string _etag { get; set; }

        [JsonProperty("_rid")]
        public string _rid { get; set; }

        [JsonProperty("_self")]
        public string _self { get; set; }
    }
}