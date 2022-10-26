using System;
using System.Linq;
using System.Numerics;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ed;
using Tide.Core;
using static Tide.Ork.Models.RandomResponse;

namespace Tide.Ork.Models {
    public class RandRegistrationReq
    {
        public RandomShareResponse[] Shares { get; set; }
        public AesKey PrismAuth { get; set; }
        public string Email { get; set; }
        public string Cmki { get; set; }
        public string Cmk2i {get;  set;}
        public string entry { get; set;}
        public DnsEntry GetEntry() => DnsEntry.Parse(entry);
        public BigInteger GetCmki() => BigInteger.Parse(Cmki);
        public BigInteger GetCmk2i() => BigInteger.Parse(Cmk2i);

        internal BigInteger ComputePrism() => Shares.Select(shr => shr.PrismVal)
            .Aggregate((sum, prism) => (sum + prism) % Ed25519.N);
        
        internal BigInteger ComputeCmk() => Shares.Select(shr => shr.CmkVal)
            .Aggregate((sum, cmk) => (sum + cmk) % Ed25519.N);

        internal BigInteger ComputeCmk2() => Shares.Select(shr => shr.Cmk2Val)
            .Aggregate((sum, cmk2) => (sum + cmk2) % Ed25519.N);
    }
}