using System;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ecc;
using Tide.VendorSdk.Classes;

namespace Tide.Vendor {
    public class Settings
    {
        public AdminKeys Keys { get; set; }
    }

    public class AdminKeys
    {
        public string PrivateKey { get; set; }
        public string SecretKey { get; set; }

        public VendorConfig CreateVendorConfig() {
            return new VendorConfig {
                PrivateKey = C25519Key.Parse(Convert.FromBase64String(PrivateKey)),
                SecretKey = AesKey.Parse(SecretKey)
            };
        }
    }
}