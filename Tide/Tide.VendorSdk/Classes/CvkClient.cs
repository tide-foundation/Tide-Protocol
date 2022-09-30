using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Tide.Encryption.AesMAC;
using System.Text.Json;
using Tide.Encryption.Ecc;
using Tide.Encryption.Ed;
using System.Net;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tide.VendorSdk.Classes
{
    public class CvkClient
    {
        private IdGenerator _idGen;
        private readonly HttpClient _client;

        public CvkClient(Uri uri)
        {
            _idGen = null;
            _client = new HttpClient { BaseAddress = uri };
        }

        public async Task<BigInteger> GetId()
        {
            if (_idGen == null)
                await SetIdGen();
            
            return _idGen.Id;
        }

        public async Task<Guid> GetGuid()
        {
            if (_idGen == null)
                await SetIdGen();

            return _idGen.Guid;
        }

        public async Task Add(Guid viud, BigInteger cvki, AesKey cvkiAuth, Tide.Encryption.Ed.Ed25519Key cvkPub, Guid keyId, byte[] signature)
        {
            var payload = JsonSerializer.Serialize(new[] {
                cvkPub.ToByteArray(),
                cvki.ToByteArray(true, true),
                cvkiAuth.ToByteArray(),
                signature,
            });

            var body = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"api/cvk/{viud}/{keyId}", body);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException(response.ToString());
        }

        public async Task<Tide.Encryption.Ed.Ed25519Key> GetPublic()
        {
            var response = await _client.GetAsync("api/public");

            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException(response.ToString());

            var keyText = await response.Content.ReadAsStringAsync();
            return Tide.Encryption.Ed.Ed25519Key.Parse(Convert.FromBase64String(keyText));
        }

        public async Task<(byte[] Token, C25519Cipher Challenge)> Challenge(Guid viud, Guid keyId)
        {
            var response = await _client.GetAsync($"api/cvk/challenge/{viud}/{keyId}");

            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException(response.ToString());

            var res = JsonSerializer.Deserialize<Dictionary<string, byte[]>>(await response.Content.ReadAsStringAsync());
            return (res["token"], C25519Cipher.Parse(res["challenge"]));
        }

        public async Task<List<byte[]>> DecryptBulk(Guid viud, Guid keyId, IEnumerable<byte[]> data, byte[] token, byte[] sign)
        {
            var bodyContent = string.Join("\\n", data.Select(dta => Convert.ToBase64String(dta)));
            var body = new StringContent($"\"{bodyContent}\"", Encoding.UTF8, "application/json");
            var tkn = WebEncoders.Base64UrlEncode(token);
            var sgn = WebEncoders.Base64UrlEncode(sign);

            var response = await _client.PostAsync($"api/cvk/plaintext/{viud}/{keyId}/{tkn}/{sgn}", body);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException(response.ToString());

            return Regex.Split(await response.Content.ReadAsStringAsync(), @"\r?\n")
                .Select(res => Convert.FromBase64String(res.Trim())).ToList();
        }

        public async Task<byte[]> Decrypt(Guid viud, Guid keyId, byte[] data, byte[] token, byte[] sign)
        {
            var body = new StringContent($"\"{Convert.ToBase64String(data)}\"", Encoding.UTF8, "application/json");
            var tkn = WebEncoders.Base64UrlEncode(token);
            var sgn = WebEncoders.Base64UrlEncode(sign);

            var response = await _client.PostAsync($"api/cvk/plaintext/{viud}/{keyId}/{tkn}/{sgn}", body);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException(response.ToString());
            
            return Convert.FromBase64String(await response.Content.ReadAsStringAsync());
        }

        private async Task SetIdGen()
        {
            var key = await GetPublic();
            _idGen = IdGenerator.Seed(key.ToByteArray());
        }

        static string ToBase64Url(byte[] data)
        {
            return Convert.ToBase64String(data).Replace("/", "_").Replace("+", "-");
        }
    }
}