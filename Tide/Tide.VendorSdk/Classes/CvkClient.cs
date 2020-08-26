﻿using System;
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

        public async Task Add(Guid viud, BigInteger cvki, AesKey cvkiAuth, C25519Key cvkPub)
        {
            var payload = JsonSerializer.Serialize(new[] {
                cvkPub.ToByteArray(),
                cvki.ToByteArray(true, true),
                cvkiAuth.ToByteArray(),
            });

            var body = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"api/cvk/{viud}", body);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException(response.RequestMessage.ToString());
        }

        public async Task<C25519Key> GetPublic()
        {
            var response = await _client.GetAsync("api/public");

            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException(response.RequestMessage.ToString());

            var keyText = await response.Content.ReadAsStringAsync();
            return C25519Key.Parse(Convert.FromBase64String(keyText));
        }

        public async Task<(byte[] Token, C25519Cipher Challenge)> Challenge(Guid viud, Guid keyId)
        {
            var response = await _client.GetAsync($"api/cvk/challenge/{viud}/{keyId}");

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

            var response = await _client.GetAsync($"api/cvk/plaintext/{viud}/{keyId}/{dta}/{tkn}/{sgn}");

            if (response.StatusCode != HttpStatusCode.OK)
                throw new HttpRequestException(response.RequestMessage.ToString());
            
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