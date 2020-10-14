using System;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ecc;
using Tide.VendorSdk.Classes;

namespace Tide.Vendor {
    public class Settings
    {
        public AdminKeys Keys { get; set; }
        public string BearerKey { get; set; }
        public bool DevFront { get; set; }
    }

   
}