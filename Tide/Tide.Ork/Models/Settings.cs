using System;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ecc;

namespace Tide.Ork.Models {
    public class Settings
    {
        public Instance Instance { get; set; }
        public Endpoints Endpoints { get; set; }
        public EmailClient EmailClient { get; set; }
        public bool Memory { get; set; }
    }

    public class Instance
    {
        public string PrivateKey { get; set; }
        public string SecretKey { get; set; }
        public string Username { get; set; }

        public C25519Key GetPrivateKey() => C25519Key.Parse(Convert.FromBase64String(PrivateKey));
        public AesKey GetSecretKey() => AesKey.Parse(SecretKey);
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

    public class EmailClient
    {
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
        public string SenderHost { get; set; }
        public int SenderPort { get; set; }
    }
}