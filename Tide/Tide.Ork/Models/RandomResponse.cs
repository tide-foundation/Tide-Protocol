using System;
using System.Diagnostics;
using System.Numerics;
using Tide.Encryption.Ecc;
using Tide.Encryption.SecretSharing;

namespace Tide.Ork.Models {
    public class RandomResponse
    {
        public Guid Id { get; set; }
        public byte[] Prism { get; set; }
        public byte[] Cmk { get; set; }
        public C25519Point Password { get; set; }

        private readonly Lazy<BigInteger> _prismVal;
        internal BigInteger PrismVal => _prismVal.Value;
        
        private readonly Lazy<BigInteger> _cmkVal;
        internal BigInteger CmkVal => _cmkVal.Value;

        public RandomResponse() {
            _prismVal = new Lazy<BigInteger>(() => new BigInteger(Prism, true, true));
            _cmkVal = new Lazy<BigInteger>(() => new BigInteger(Cmk, true, true));
        }

        public RandomResponse(C25519Point pass, Point prism, Point cmk): this() {
            Debug.Assert(prism?.X == cmk?.X, $"{nameof(prism)} and {nameof(cmk)} must be the same");

            Id = new Guid(prism.X.ToByteArray(true, true));
            Prism = prism.Y.ToByteArray(true, true);
            Cmk = cmk.Y.ToByteArray(true, true);
            Password = pass;
        }
    }
}