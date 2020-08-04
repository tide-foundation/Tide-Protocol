using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Tide.Encryption.AesMAC;
using System.Text.Json;
using Tide.Encryption.Ecc;
using System.Net;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;

namespace Tide.VendorSdk.Classes
{
    public class OrkClient
    {
        private readonly IdGenerator _idGen;
        private readonly HttpClient _client;

        public BigInteger Id { get => _idGen.Id; }
        public Guid Guid { get => _idGen.Guid; }

        public OrkClient(Uri uri)
        {
            _idGen = IdGenerator.Seed(uri);
            _client = new HttpClient { BaseAddress = uri };
        }

        public async Task RegisterCvk(Guid viud, BigInteger cvki, AesKey cvkAuthi, C25519Key pub)
        {
            var payload = JsonSerializer.Serialize(new[] {
                pub.ToByteArray(),
                cvki.ToByteArray(true, true),
                cvkAuthi.ToByteArray(),
            });

            var body = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"api/dauth/{viud}/cvk", body);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException(response.RequestMessage.ToString());
        }

        public async Task<(byte[] Token, C25519Cipher Challenge)> Challenge(Guid viud, Guid keyId)
        {
            var response = await _client.GetAsync($"api/dauth/{viud}/challenge/{keyId}");

            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException(response.RequestMessage.ToString());

            var res = JsonSerializer.Deserialize<Dictionary<string, byte[]>>(await response.Content.ReadAsStringAsync());
            return (res["token"], C25519Cipher.Parse(res["challenge"]));
        }

        public async Task<byte[]> Decrypt(Guid viud, Guid keyId, byte[] data, byte[] token, byte[] sign)
        {
            var dta = WebEncoders.Base64UrlEncode(data);
            var tkn = WebEncoders.Base64UrlEncode(token);
            var sgn = WebEncoders.Base64UrlEncode(sign);

            var response = await _client.GetAsync($"api/dauth/{viud}/decrypt/{keyId}/{dta}/{tkn}/{sgn}");

            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException(response.RequestMessage.ToString());
            
            return Convert.FromBase64String(await response.Content.ReadAsStringAsync());
        }

        static string ToBase64Url(byte[] data)
        {
            return Convert.ToBase64String(data).Replace("/", "_").Replace("+", "-");
        }
    }
}