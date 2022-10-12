using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net;
using Tide.Core;
using Tide.Encryption.Ed;
using System.Text;

namespace Tide.VendorSdk.Classes
{
    public class KeyClient
    {
        private readonly HttpClient _client;

        public KeyClient(Uri uri)
        {
            _client = new HttpClient { BaseAddress = uri };
        }

        public async Task<KeyIdVault> Get(Guid uid)
        {
            var resp = await _client.GetAsync($"api/key/{uid}");
            if (resp.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!resp.IsSuccessStatusCode) throw new HttpRequestException(resp.ToString());

            return JsonSerializer.Deserialize<KeyIdVaultDTO>(await resp.Content.ReadAsStringAsync(),
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        public async Task SetOrUpdate(KeyIdVault key)
        {
            var body = new StringContent(JsonSerializer.Serialize<KeyIdVaultDTO>(key), Encoding.UTF8, "application/json");
            var resp = await _client.PostAsync($"api/key", body);
            
            if (!resp.IsSuccessStatusCode) throw new HttpRequestException(resp.ToString());
        }

        private class KeyIdVaultDTO
        {
            public Guid KeyId { get; set; }
            public string Key { get; set; }

            public KeyIdVaultDTO() { }

            public KeyIdVaultDTO(KeyIdVault vault)
            {
                KeyId = vault.KeyId;
                Key = Convert.ToBase64String(vault.Key.ToByteArray());
            }

            public KeyIdVault Map()
            {
                return new KeyIdVault
                {
                    KeyId = this.KeyId,
                    Key = Ed25519Key.ParsePublic(Convert.FromBase64String(Key))
                };
            }

            public static implicit operator KeyIdVault(KeyIdVaultDTO k) => k.Map();
            public static implicit operator KeyIdVaultDTO(KeyIdVault k) => new KeyIdVaultDTO(k);
        }
    }
}
