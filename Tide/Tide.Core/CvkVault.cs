using System;
using System.Collections.Generic;
using System.Numerics;
using Tide.Encryption;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ed;

namespace Tide.Core
{
    public class CvkVault : SerializableByteBase<CvkVault>, IGuid
    {
        public Guid Id => VuId;
        public Guid VuId { get; set; }
        public Ed25519Key CvkPub { get; set; }
        public BigInteger CVKi { get; set; }
        public AesKey CvkiAuth { get; set; }

        public CvkVault() : base(1)
        {
            CvkiAuth = new AesKey();
        }

        protected override IEnumerable<byte[]> GetItems()
        {
            yield return VuId.ToByteArray();
            yield return CvkPub != null ? CvkPub.ToByteArray() : new byte[] { };
            yield return CVKi.ToByteArray(true, true);
            yield return CvkiAuth.ToByteArray();
        }

        protected override void SetItems(IReadOnlyList<byte[]> data)
        {
            VuId = new Guid(data[0]);
            CvkPub = data[1].Length != 0 ? Ed25519Key.Parse(data[1]) : null;
            CVKi = new BigInteger(data[2], true, true);
            CvkiAuth = AesKey.Parse(data[3]);
        }
    }
}
