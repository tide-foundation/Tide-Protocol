using System;
using System.Collections.Generic;
using Tide.VendorSdk.Classes;

namespace Tide.Vendor {
    public class Settings
    {
        public AdminKeys Keys { get; set; }
        public string BearerKey { get; set; }
        public string Audience { get; set; }
        public bool DevFront { get; set; }
        public List<string> OrkUrls { get; set; }
    }

   
}