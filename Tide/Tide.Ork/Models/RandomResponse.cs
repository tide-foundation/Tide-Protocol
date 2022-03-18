using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Tide.Encryption.Ecc;
using Tide.Encryption.SecretSharing;

namespace Tide.Ork.Models {
    public class RandomResponse
    {
        public C25519Point Password { get; set; }
        public C25519Point CmkPub { get; set; }
        public C25519Point VendorCMK { get; set; }
        public RandomShareResponse[] Shares { get; set; }

        public RandomResponse() { }

        public RandomResponse(C25519Point pass, C25519Point pub, C25519Point vendorCMK, IReadOnlyList<Point> prisms, IReadOnlyList<Point> cmks)
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