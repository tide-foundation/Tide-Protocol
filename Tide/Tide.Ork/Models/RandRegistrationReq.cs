using System;
using System.Linq;
using System.Numerics;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ecc;

namespace Tide.Ork.Models {
    public class RandRegistrationReq
    {
        public RandomResponse[] Shares { get; set; }
        public AesKey PrismAuth { get; set; }
        public string Email { get; set; }

        internal BigInteger ComputePrism() => Shares.Select(shr => shr.PrismVal)
            .Aggregate((sum, prism) => (sum + prism) % C25519Point.N);
        
        internal BigInteger ComputeCmk() => Shares.Select(shr => shr.CmkVal)
            .Aggregate((sum, cmk) => (sum + cmk) % C25519Point.N);
    }
}