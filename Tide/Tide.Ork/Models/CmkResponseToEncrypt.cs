using System;
using System.Numerics;
using System.Text.Json;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Tide.Encryption.Ed;
using Tide.Encryption.SecretSharing;

namespace Tide.Ork.Models {
     public class CmkResponseToEncrypt
    {
        public byte[] gCMKi {get; set;} 
        public byte [] gPRISMi {get; set;}
        public byte [] gCMK2i {get; set;}
        public CMKResponseShare[] Shares {get; set;}
          public CmkResponseToEncrypt(IReadOnlyList<Point> cmks,IReadOnlyList<Point> prisms, IReadOnlyList<Point> cmk2s, Ed25519Point gCmki, Ed25519Point gPrismi, Ed25519Point gCmk2i)
        {
            Debug.Assert(prisms != null && cmks != null && cmk2s!= null && prisms.Any() && cmks.Any() && cmk2s.Any(), $"Argument cannot be empty");
            Debug.Assert(prisms.Count == cmks.Count, $"{nameof(prisms)} and {nameof(cmks)} must be the same");
            Debug.Assert(prisms.Count == cmk2s.Count, $"{nameof(prisms)} and {nameof(cmk2s)} must be the same");
            Debug.Assert(!prisms.Where((_, i) => prisms[i].X != cmks[i].X).Any(), $"{nameof(prisms)} and {nameof(cmks)} must be the same");
            Debug.Assert(!prisms.Where((_, i) => prisms[i].X != cmk2s[i].X).Any(), $"{nameof(prisms)} and {nameof(cmk2s)} must be the same");

            gCMKi = gCmki.ToByteArray();
            gPRISMi = gPrismi.ToByteArray();
            gCMK2i = gCmk2i.ToByteArray();
            Shares = prisms.Select((_, i) => new CMKResponseShare {
                CMKYj = cmks[i].Y.ToByteArray(true, true),
                PRISMYj = prisms[i].Y.ToByteArray(true, true),
                CMK2Yj = cmk2s[i].Y.ToByteArray(true, true)
            }).ToArray();
            
        }

        // doing this because the size of ed25519 points will change in future
        public string ToJSON() => JsonSerializer.Serialize(this);
            

    }

    public class CMKResponseShare {
        public byte[] CMKYj { get; set; }
        public byte[] PRISMYj { get; set; }
        public byte[] CMK2Yj  { get; set; }

        internal BigInteger PrismjVal => new BigInteger(CMKYj, true, true);
        internal BigInteger CmkjVal => new BigInteger(PRISMYj, true, true);
        internal BigInteger Cmk2jVal => new BigInteger(CMK2Yj , true, true);
    }
}