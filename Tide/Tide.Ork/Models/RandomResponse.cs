using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Tide.Encryption.Ed;
using Tide.Encryption.SecretSharing;

namespace Tide.Ork.Models {
    public class RandomResponse
    {
        public Ed25519Point Password { get; set; }
        public Ed25519Point CmkPub { get; set; }
        public Ed25519Point VendorCMK { get; set; }
        public RandomShareResponse[] Shares { get; set; }

        public RandomResponse() { }

        public RandomResponse(Ed25519Point pass, Ed25519Point pub, Ed25519Point vendorCMK, IReadOnlyList<Point> prisms, IReadOnlyList<Point> cmks)
        {
            Debug.Assert(prisms != null && cmks != null && prisms.Any() && cmks.Any(), $"Argument cannot be empty");
            Debug.Assert(prisms.Count == cmks.Count, $"{nameof(prisms)} and {nameof(cmks)} must be the same");
            Debug.Assert(!prisms.Where((_, i) => prisms[i].X != cmks[i].X).Any(), $"{nameof(prisms)} and {nameof(cmks)} must be the same");

            Password = pass;
            CmkPub = pub;
            VendorCMK = vendorCMK;
            Shares = prisms.Select((_, i) => new RandomShareResponse {
                Id = new Guid(cmks[i].X.ToByteArray(true, true)),
                Prism = prisms[i].Y.ToByteArray(true, true),
                Cmk = cmks[i].Y.ToByteArray(true, true)
            }).ToArray();
            
        }

        public class RandomShareResponse
        {
            public Guid Id { get; set; }
            public byte[] Prism { get; set; }
            public byte[] Cmk { get; set; }

            internal BigInteger PrismVal => new BigInteger(Prism, true, true);
            internal BigInteger CmkVal => new BigInteger(Cmk, true, true);
        }
    }
}