using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using Tide.Encryption.Ecc;
using Tide.Encryption.Tools;

namespace Tide.Core
{
    public class DnsEntry : SerializableJson<DnsEntry, DnsEntry>, IGuid
    {
        public Guid Id { get; set; }
        public string[] Orks { get; set; }
        public string Public { get; set; }
        public long Modifided { get; set; }
        public string Signature { get; set; }
        public string[] Signatures { get; set; }
        public string[] Urls { get; set; }
        public string[] Publics { get; set; }

        public bool VerifyForUId() {
            if (string.IsNullOrEmpty(Signature) || string.IsNullOrEmpty(Public))
                return false;

            return GetPublicKey().Verify(MessageSigned(), Convert.FromBase64String(Signature));
        }

        public List<Uri> GetUrls() => Urls.Where(url => !string.IsNullOrWhiteSpace(url))
            .Select(url => new Uri(url)).ToList();

        public List<C25519Key> GetPublics() => Publics.Where(pub => !string.IsNullOrWhiteSpace(pub))
            .Select(pub => C25519Key.Parse(pub.Trim())).ToList();

        public C25519Key GetPublicKey() {
            return C25519Key.Parse(Public);
        }
        
        public byte[] MessageSigned() {
            return Utils.Hash(JsonSerializer.Serialize(new { Id, Orks, Public, Modifided }, GetJsonOptions()));
        }
        
        protected override JsonSerializerOptions GetJsonOptions()
        {
            return new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        protected override DnsEntry GetDto() => this;

        protected override void MapDto(DnsEntry basic)
        {
            Id = basic.Id;
            Orks = basic.Orks;
            Public = basic.Public;
            Modifided = basic.Modifided;
            Signature = basic.Signature;
            Signatures = basic.Signatures;
        }
    }
}
