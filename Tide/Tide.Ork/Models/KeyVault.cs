using System;
using System.Numerics;
using Tide.Encryption.AesMAC;

//TODO: This sould be in library
namespace Tide.Ork.Models {
    public class KeyVault {
        public Guid User { get; set; }
        public BigInteger AuthShare { get; set; }
        public BigInteger KeyShare { get; set; }
        public AesKey Secret { get; set; }
    }
}