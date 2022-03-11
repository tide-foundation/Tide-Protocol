using System;
using System.Numerics;
using Tide.Encryption.Ecc;
using Tide.Ork.Models;
using Tide.VendorSdk.Classes;

namespace Tide.Ork.Classes {
    public class OrkConfig {
        private readonly IdGenerator _IdGen;

        public int Threshold { get; }

        public string UserName { get; }

        public C25519Key PrivateKey { get; }

        public BigInteger Id => _IdGen.Id;

        public Guid Guid => _IdGen.Guid;

        public OrkConfig(Settings settings)
        {
            Threshold = settings.Instance.Threshold;
            UserName = settings.Instance.Username;
            PrivateKey = settings.Instance.GetPrivateKey();
            _IdGen = IdGenerator.Seed(PrivateKey.GetPublic().ToByteArray());
        }
    }
}