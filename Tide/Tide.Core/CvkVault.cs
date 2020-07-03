using System;
using System.Collections.Generic;
using System.Numerics;
using Tide.Encryption;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ecc;

namespace Tide.Core
{
    public class CvkVault : SerializableByteBase
    {
        public Guid User { get; set; }
        public C25519Key VendorPub { get; set; }
        public BigInteger CVKi { get; set; }
        public AesKey CvkAuth { get; set; }

        public CvkVault() : base(1)
        {
            CvkAuth = new AesKey();
        }

        protected override IEnumerable<byte[]> GetItems()
        {
            yield return User.ToByteArray();
            yield return VendorPub != null ? VendorPub.ToByteArray() : new byte[]{};
            yield return CVKi.ToByteArray(true, true);
            yield return CvkAuth.ToByteArray();
        }

        protected override void SetItems(IReadOnlyList<byte[]> data)
        {
            User = new Guid(data[0]);
            VendorPub = data[1].Length != 0 ? C25519Key.Parse(data[1]) : null;
            CVKi = new BigInteger(data[2], true, true);
            CvkAuth = AesKey.Parse(data[3]);
        }

        public static CvkVault Parse(string data) => Serializer.Parse<CvkVault>(data);

        public static CvkVault Parse(byte[] data) => Serializer.Parse<CvkVault>(data);
    }
}
