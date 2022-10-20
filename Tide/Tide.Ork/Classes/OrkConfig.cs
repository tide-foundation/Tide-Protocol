using System;
using System.Numerics;
using Tide.Encryption.Ed;
using Tide.Ork.Models;
using Tide.VendorSdk.Classes;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Classes {
    public class OrkConfig {
        private readonly IdGenerator _IdGen;
        public int Threshold { get; }
        public string UserName { get; }

        public Ed25519Key PrivateKey { get; }

        public BigInteger Id => _IdGen.Id;

        public Guid Guid => _IdGen.Guid;
        public AesKey SecretKey { get;}

        public OrkConfig(Settings settings)
        {
            UserName =  settings.Instance.Username;
            //Threshold = settings.Instance.Threshold;
            PrivateKey = settings.Instance.GetPrivateKey();
            SecretKey = settings.Instance.GetSecretKey();
            _IdGen = IdGenerator.Seed(PrivateKey.GetPublic().ToByteArray());
        }
    }
}