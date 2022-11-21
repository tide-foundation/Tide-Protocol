using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Tide.Encryption.Ed;
using Tide.Encryption.SecretSharing;
using System.Text.Json;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Models
{
    public class CVKResponseToEncrypt
    {
        public DataToDecrypt[] Shares { get; set; }
        public string[] To_Orks {get; set;}
        public string From_Ork {get; set;}
        public ShareEncrypted[] ShareEncrypted {get; set;}
        public CVKResponseToEncrypt() {}

        public CVKResponseToEncrypt( Ed25519Point cvkPub, Ed25519Point cvk2Pub, IReadOnlyList<Point> cvks, IReadOnlyList<Point> cvk2s, string[] toOrks , string fromOrk)
        {
            Debug.Assert(cvks != null && cvk2s != null  && cvks.Any() && cvk2s.Any() , $"Argument cannot be empty");
            Debug.Assert(cvks.Count == cvk2s.Count, $"{nameof(cvks)} and {nameof(cvk2s)} must be the same");
            Debug.Assert(!cvks.Where((_, i) => cvks[i].X != cvk2s[i].X).Any(), $"{nameof(cvks)} and {nameof(cvk2s)} must be the same");

            To_Orks =toOrks;
            From_Ork = fromOrk;
            Shares = cvks.Select((_, i) => new DataToDecrypt
            {
                gCVKi = cvkPub.ToByteArray(),
                gCVK2i =cvk2Pub.ToByteArray(),
                CVKYj = cvks[i].Y.ToByteArray(true, true),
                CVK2Yj = cvk2s[i].Y.ToByteArray(true, true)
            }).ToArray();
        }

        public void GetCVKShares(AesKey[] ECDHKeys){
            this.ShareEncrypted = Shares.Select((share,i) => new ShareEncrypted{
                To = this.To_Orks[i],
                From = this.From_Ork,
                EncryptedData = ECDHKeys[i].EncryptStr(share.ToJSON())
            }).ToArray();
        }

        public string ToJSON() {
            var toReturn = new {
                ShareEncrypted = this.ShareEncrypted
            };
            return JsonSerializer.Serialize(toReturn);
        } 
        public class CVKShareEncrypted
        {
           public string To {get; set;}
           public string From {get; set;}
           public string EncryptedData {get; set;}
        }

        public class DataToDecrypt{
            public byte[] gCVKi { get; set; }
            public byte[] gCVK2i {get; set;}
            public byte[] CVKYj { get; set; }
            public byte[] CVK2Yj  {get; set;}
            public string ToJSON() => JsonSerializer.Serialize(this);
            internal BigInteger CvkjVal => new BigInteger(CVKYj, true, true);
            internal BigInteger Cvk2jVal => new BigInteger(CVK2Yj , true, true);
        
        }

    }
}
