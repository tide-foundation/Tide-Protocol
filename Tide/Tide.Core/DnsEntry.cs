using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using Tide.Encryption.Ed;
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
        public string MessageToSign(){
            var msg = new[]
            { 
                new { id = Id.ToString(), orks = Orks, Public = Public, modified = Modifided}
            };
            return JsonSerializer.Serialize(msg);
        }

        public bool VerifyForUId() {

            if (string.IsNullOrEmpty(Signature) || string.IsNullOrEmpty(Public))
                return false;
            return GetPublicKey().EdDSAVerify(MessageSignedBytes(), Convert.FromBase64String(Signature));
        }

        public List<Uri> GetUrls() => Urls.Where(url => !string.IsNullOrWhiteSpace(url))
            .Select(url => new Uri(url)).ToList();

        public List<Ed25519Key> GetPublics() => Publics.Where(pub => !string.IsNullOrWhiteSpace(pub))
            .Select(pub => Ed25519Key.ParsePublic(pub.Trim())).ToList();

        public Ed25519Key GetPublicKey() {
            return Ed25519Key.ParsePublic(Public);
        }
        
        public byte[] MessageSigned() {
            return Utils.Hash(JsonSerializer.Serialize(new { Id, Orks, Public, Modifided }, GetJsonOptions()));
        }

        public byte[] MessageSignedSHA512() {
            return Utils.HashSHA512(JsonSerializer.Serialize(new { Id, Orks, Public, Modifided }, GetJsonOptions()));
        }

        public byte[] MessageSignedBytes() {
            return System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { Id, Orks, Public, Modifided }, GetJsonOptions()));
        }
        
        protected override JsonSerializerOptions GetJsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
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
