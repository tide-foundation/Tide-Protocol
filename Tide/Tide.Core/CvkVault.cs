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
        public Ed25519Point GCVK { get; set; }
        public BigInteger CVKi { get; set; }
        public AesKey GCvkAuth { get; set; }
        public BigInteger CVK2i { get; set; }
        public Ed25519Point GCVK2 { get; set; }

        public CvkVault() 
        {
           
        }

        protected override IEnumerable<byte[]> GetItems()
        {
            yield return VuId.ToByteArray();
            yield return GCVK != null ? GCVK.ToByteArray() : new byte[] { };
            yield return CVKi.ToByteArray(true, true);
            yield return GCvkAuth.ToByteArray();
            yield return CVK2i.ToByteArray(true, true);
            yield return GCVK2 != null ? GCVK2.ToByteArray() : new byte[] { };
        }

        protected override void SetItems(IReadOnlyList<byte[]> data)
        {
            VuId = new Guid(data[0]);
            GCVK = data[1].Length != 0 ? Ed25519Point.From(data[1]) : null;
            CVKi = new BigInteger(data[2], true, true);
            GCvkAuth = AesKey.Parse(data[3]);
            CVK2i = new BigInteger(data[4], true, true);
            GCVK2= data[5].Length != 0 ? Ed25519Point.From(data[5]) : null;
        }
    }
}
