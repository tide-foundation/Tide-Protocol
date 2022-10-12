using System;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ed;

namespace Tide.VendorSdk.Classes
{
    public class AdminKeys
    {
        public string PrivateKey { get; set; }
        public string SecretKey { get; set; }

        public VendorConfig CreateVendorConfig()
        {
            return new VendorConfig
            {
                PrivateKey = Ed25519Key.Parse(Convert.FromBase64String(PrivateKey)),
                SecretKey = AesKey.Parse(SecretKey)
            };
        }
    }
}
