using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Tide.Encryption;
using Tide.Encryption.Ed;
using Tide.Encryption.AesMAC;

namespace Tide.Core
{
    public class CmkVault : SerializableByteBase<CmkVault>, IGuid
    {
        public Guid Id => UserId;
        public Guid UserId { get; set; }
        public Ed25519Point GCmk {get; set;}
        public BigInteger Cmki { get; set; }
        public BigInteger Prismi { get; set; }       
        public AesKey PrismAuthi { get; set; }
        public BigInteger Cmk2i { get; set; }
         public Ed25519Point GCmk2 {get; set;}
        public string Email { get; set; }
        public string CommitStatus {get; set;}
        

        public CmkVault() : base(1)
        {
            
        }

        protected override IEnumerable<byte[]> GetItems()
        {
            yield return UserId.ToByteArray();
            yield return GCmk.ToByteArray();
            yield return Cmki.ToByteArray(true, true);
            yield return Prismi.ToByteArray(true, true);
            yield return PrismAuthi.ToByteArray();
            yield return Cmk2i.ToByteArray(true, true);
            yield return GCmk2.ToByteArray();
            yield return Encoding.UTF8.GetBytes(Email);
            
        }

        protected override void SetItems(IReadOnlyList<byte[]> data)
        {
            UserId = new Guid(data[0]);
            GCmk = Ed25519Point.From(data[1]);
            Cmki = new BigInteger(data[2], true, true);
            Prismi = new BigInteger(data[3], true, true);    
            PrismAuthi = AesKey.Parse(data[4]);
            Cmk2i = new BigInteger(data[5], true, true);
            GCmk2 = Ed25519Point.From(data[6]);
            Email =  Encoding.UTF8.GetString(data[7]);
            CommitStatus = Encoding.UTF8.GetString(data[8]);
           
        }
    }
}
