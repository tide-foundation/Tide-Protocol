using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Tide.Encryption.Ed;
using Tide.Encryption.SecretSharing;
using System.Text.Json;

namespace Tide.Ork.Models
{
    public class CVKResponseToEncrypt
    {
        public byte[] gCVKi { get; set; }

        public byte[] gCVK2i {get; set;}
        public CVKResponseShare[] Shares { get; set; }
        public CVKResponseToEncrypt() {}

        public CVKResponseToEncrypt( Ed25519Point cvkPub, Ed25519Point cvk2Pub, IReadOnlyList<Point> cvks, IReadOnlyList<Point> cvk2s )
        {
            Debug.Assert(cvks != null && cvk2s != null  && cvks.Any() && cvk2s.Any() , $"Argument cannot be empty");
            Debug.Assert(cvks.Count == cvk2s.Count, $"{nameof(cvks)} and {nameof(cvk2s)} must be the same");
            Debug.Assert(!cvks.Where((_, i) => cvks[i].X != cvk2s[i].X).Any(), $"{nameof(cvks)} and {nameof(cvk2s)} must be the same");

            gCVKi = cvkPub.ToByteArray();
            gCVK2i =cvk2Pub.ToByteArray();
            Shares = cvks.Select((_, i) => new CVKResponseShare
            {
                CVKYj = cvks[i].Y.ToByteArray(true, true),
                CVK2Yj = cvk2s[i].Y.ToByteArray(true, true)
            }).ToArray();
        }

       // doing this because the size of ed25519 points will change in future
        public string ToJSON() => JsonSerializer.Serialize(this);

        public class CVKResponseShare
        {
            public byte[] CVKYj { get; set; }
            public byte[] CVK2Yj  {get; set;}
            internal BigInteger CvkjVal => new BigInteger(CVKYj, true, true);
            internal BigInteger Cvk2jVal => new BigInteger(CVK2Yj , true, true);
        }
    }
}
