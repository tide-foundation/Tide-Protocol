using System;
using System.Collections.Generic;
using System.Text;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ecc;

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
                PrivateKey = C25519Key.Parse(Convert.FromBase64String(PrivateKey)),
                SecretKey = AesKey.Parse(SecretKey)
            };
        }
    }
}
