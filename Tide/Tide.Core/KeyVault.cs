using System;
using System.Collections.Generic;
using System.Numerics;
using Tide.Encryption;
using Tide.Encryption.AesMAC;

namespace Tide.Core
{
    public class KeyVault : SerializableByteBase
    {
        public Guid User { get; set; }
        public BigInteger AuthShare { get; set; }
        public BigInteger KeyShare { get; set; }
        public AesKey Secret { get; set; }

        public KeyVault() : base(1)
        {
            Secret = new AesKey();
        }

        protected override IEnumerable<byte[]> GetItems()
        {
            yield return User.ToByteArray();
            yield return AuthShare.ToByteArray(true, true);
            yield return KeyShare.ToByteArray(true, true);
            yield return Secret.ToByteArray();
        }

        protected override void SetItems(IReadOnlyList<byte[]> data)
        {
            User = new Guid(data[0]);
            AuthShare = new BigInteger(data[1], true, true);
            KeyShare = new BigInteger(data[2], true, true);
            Secret = AesKey.Parse(data[3]);
        }

        public static KeyVault Parse(string data) => Serializer.Parse<KeyVault>(data);

        public static KeyVault Parse(byte[] data) => Serializer.Parse<KeyVault>(data);
    }
}
