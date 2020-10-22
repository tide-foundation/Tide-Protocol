using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tide.Encryption.Ecc;
using Tide.Encryption.Tools;

namespace Tide.Core
{
    public class DnsEntry : IGuid
    {
        [JsonIgnore]
        public Guid Id => UId;
        public Guid UId { get; set; }
        public string[] Orks { get; set; }
        public string Public { get; set; }
        public long Modifided { get; set; }
        public string Signature { get; set; }
        public string[] Signatures { get; set; }

        
        public bool VerifyForUId() {
            if (string.IsNullOrEmpty(Signature) || string.IsNullOrEmpty(Public))
                return false;

            return GetPublicKey().Verify(MessageSigned(), Convert.FromBase64String(Signature));
       }
        
        public C25519Key GetPublicKey() {
            return C25519Key.Parse(Public);
        }
        
        public byte[] MessageSigned() {
            return Utils.Hash(JsonSerializer.Serialize(new { UId, Orks, Public, Modifided }, GetJsonOptions()));
        }
        
        public override string ToString()
        {
            MessageSigned();
            return JsonSerializer.Serialize(this, GetJsonOptions());
        }

        public static DnsEntry Parse(string data)
        {
            return JsonSerializer.Deserialize<DnsEntry>(data, GetJsonOptions());
        }

        private static JsonSerializerOptions GetJsonOptions()
        {
            return new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }
    }
}
