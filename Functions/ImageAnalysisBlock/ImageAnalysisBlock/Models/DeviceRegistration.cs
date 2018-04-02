using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageAnalysisBlock.Models
{
    public class DeviceRegistration
    {
        public string Platform { get; set; }
        public string PnsIdentifier { get; set; }
        public string DeviceIdentifier { get; set; }
        public string[] Tags { get; set; }
    }
}
