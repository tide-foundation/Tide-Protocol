using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net;
using Tide.Core;
using System.Collections.Generic;
using System.Linq;
using Tide.Encryption.Ecc;

namespace Tide.VendorSdk.Classes
{
    public class DnsClient
    {
        private readonly HttpClient _client;

        public DnsClient(Uri uri)
        {
            _client = new HttpClient { BaseAddress = uri };
        }

        public async Task<DnsEntry> GetEntry(Guid uid)
        {
            var resp = await _client.GetAsync($"api/dns/{uid}");
            if (resp.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (!resp.IsSuccessStatusCode) throw new HttpRequestException(resp.ToString());

            return JsonSerializer.Deserialize<DnsEntry>(await resp.Content.ReadAsStringAsync(),
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> Exist(Guid uid) => await GetEntry(uid) != null;

        public async Task<(List<Uri>, List<C25519Key>)> GetInfo(Guid uid)
        {
            var entry =  await GetEntry(uid);
            if (entry == null)
                return (new List<Uri>(), new List<C25519Key>());
            
            var urls = entry.Urls.Where(url => !string.IsNullOrEmpty(url))
                .Select(url => new Uri(url)).ToList();
            
            var keys = entry.Publics.Where(url => !string.IsNullOrEmpty(url))
                .Select(url => C25519Key.Parse(url)).ToList();

            return (urls, keys);
        }
    }
}