using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Tide.Encryption;
using Tide.Encryption.AesMAC;

namespace Tide.Core
{
    public class CmkVault : SerializableByteBase<CmkVault>, IGuid, ITransactionState
    {
        public Guid Id => UserId;
        public Guid UserId { get; set; }
        public BigInteger Prismi { get; set; }
        public BigInteger Cmki { get; set; }
        public AesKey PrismiAuth { get; set; }
        public AesKey CmkiAuth { get; set; }
        public string Email { get; set; }
        public TransactionState State { get; set; }

        public CmkVault() : base(1)
        {
            PrismiAuth = new AesKey();
            CmkiAuth = new AesKey();
            Email = string.Empty;
            State = TransactionState.New;
        }

        protected override IEnumerable<byte[]> GetItems()
        {
            yield return UserId.ToByteArray();
            yield return Prismi.ToByteArray(true, true);
            yield return Cmki.ToByteArray(true, true);
            yield return PrismiAuth.ToByteArray();
            yield return CmkiAuth.ToByteArray();
            yield return Encoding.UTF8.GetBytes(Email);
            yield return new[] { (byte)State };
        }

        protected override void SetItems(IReadOnlyList<byte[]> data)
        {
            UserId = new Guid(data[0]);
            Prismi = new BigInteger(data[1], true, true);
            Cmki = new BigInteger(data[2], true, true);
            PrismiAuth = AesKey.Parse(data[3]);
            CmkiAuth = AesKey.Parse(data[4]);
            Email =  Encoding.UTF8.GetString(data[5]);
            State = (TransactionState) data[6][0];
        }
    }
}
