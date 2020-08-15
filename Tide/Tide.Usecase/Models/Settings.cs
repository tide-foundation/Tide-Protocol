using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ecc;
using Tide.VendorSdk.Classes;

namespace Tide.Usecase.Models
{
    public class Settings
    {
        public Endpoints Endpoints { get; set; }
        public AdminKeys Keys { get; set; }
        public string Connection { get; set; }
    }

    public class Endpoints
    {
        public Endpoint Simulator { get; set; }
    }

    public class Endpoint
    {
        public string Api { get; set; }
        public string Password { get; set; }
    }
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
