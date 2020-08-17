using System;
using System.Collections.Generic;
using System.Numerics;
using Tide.Encryption;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ecc;

namespace Tide.Core
{
    public class CvkVault : SerializableByteBase<CvkVault>, IGuid, ITransactionState
    {
        public Guid Id => VuId;
        public Guid VuId { get; set; }
        public C25519Key CvkPub { get; set; }
        public BigInteger CVKi { get; set; }
        public AesKey CvkiAuth { get; set; }
        public TransactionState State { get; set; }

        public CvkVault() : base(1)
        {
            CvkiAuth = new AesKey();
            State = TransactionState.New;
        }

        protected override IEnumerable<byte[]> GetItems()
        {
            yield return VuId.ToByteArray();
            yield return CvkPub != null ? CvkPub.ToByteArray() : new byte[] { };
            yield return CVKi.ToByteArray(true, true);
            yield return CvkiAuth.ToByteArray();
            yield return new[] { (byte)State };
        }

        protected override void SetItems(IReadOnlyList<byte[]> data)
        {
            VuId = new Guid(data[0]);
            CvkPub = data[1].Length != 0 ? C25519Key.Parse(data[1]) : null;
            CVKi = new BigInteger(data[2], true, true);
            CvkiAuth = AesKey.Parse(data[3]);
            State = (TransactionState)data[4][0];
        }
    }
}
