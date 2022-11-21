using System.Numerics;
using System.Text.Json;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Tide.Encryption.Ed;
using Tide.Encryption.SecretSharing;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Models {

    public class ResponseEncrypted {
        public ShareEncrypted[] EncryptedShares {get; set;}
        public DataToEncrypt[] Shares {get; set;}
        public string[] To_Usernames {get; set;}
        public string From_Username {get; set;}
        public ResponseEncrypted(IReadOnlyList<Point> cmks,IReadOnlyList<Point> prisms, IReadOnlyList<Point> cmk2s, Ed25519Point gCmki, Ed25519Point gPrismi, Ed25519Point gCmk2i, string[] to_Usernames, string from_Username)
        {
            Debug.Assert(prisms != null && cmks != null && cmk2s!= null && prisms.Any() && cmks.Any() && cmk2s.Any(), $"Argument cannot be empty");
            Debug.Assert(prisms.Count == cmks.Count, $"{nameof(prisms)} and {nameof(cmks)} must be the same");
            Debug.Assert(prisms.Count == cmk2s.Count, $"{nameof(prisms)} and {nameof(cmk2s)} must be the same");
            Debug.Assert(!prisms.Where((_, i) => prisms[i].X != cmks[i].X).Any(), $"{nameof(prisms)} and {nameof(cmks)} must be the same");
            Debug.Assert(!prisms.Where((_, i) => prisms[i].X != cmk2s[i].X).Any(), $"{nameof(prisms)} and {nameof(cmk2s)} must be the same");

            To_Usernames = to_Usernames;
            From_Username = from_Username;

            Shares = prisms.Select((_, i) => new DataToEncrypt {
                gCMKi = gCmki.ToByteArray(),
                gPRISMi = gPrismi.ToByteArray(),
                gCMK2i = gCmk2i.ToByteArray(),
                CMKYj = cmks[i].Y.ToByteArray(true, true),
                PRISMYj = prisms[i].Y.ToByteArray(true, true),
                CMK2j = cmk2s[i].Y.ToByteArray(true, true)
            }).ToArray();
        }
        public void Encrypt(AesKey[] ECDHKeys){
            this.EncryptedShares = Shares.Select((share, i) => new ShareEncrypted {
                To = this.To_Usernames[i],
                From = this.From_Username,
                EncryptedData = ECDHKeys[i].EncryptStr(share.ToJSON())
             }).ToArray();
        }
        public string GetEncryptedShares() {
            if (this.EncryptedShares == null){
                throw new System.NullReferenceException("ResponseEncrypted: Shares were not encrypted beforehand. Make sure to encrypt them first");
            }
            var toReturn = new {
                EncryptedShares = this.EncryptedShares
            };
            return JsonSerializer.Serialize(toReturn);
        } 
    }
    public class ShareEncrypted {
        public string To {get; set;} /// Ork Username the share will go to
        public string From {get; set;} /// Ork Username the share is sent from
        public string EncryptedData {get; set;} // this is the DataToEncrypt object encrypted
    }
    public class DataToEncrypt{
        public byte[] CMKYj { get; set; }
        public byte[] PRISMYj { get; set; }
        public byte[] CMK2j  { get; set; }
        public byte[] gCMKi {get; set;} 
        public byte [] gPRISMi {get; set;}
        public byte [] gCMK2i {get; set;}
        public string ToJSON() => JsonSerializer.Serialize(this);
        internal BigInteger PrismjVal => new BigInteger(CMKYj, true, true);
        internal BigInteger CmkjVal => new BigInteger(PRISMYj, true, true);
        internal BigInteger Cmk2jVal => new BigInteger(CMK2j , true, true);
    }
}