using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net;
using Tide.Core;
using System.Collections.Generic;
using System.Linq;
using Tide.Encryption.Ecc;
using System.Text;

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

        public async Task<List<DnsEntry>> GetEntries(IEnumerable<Guid> uids)
        {
            var body = new StringContent(JsonSerializer.Serialize(uids), Encoding.UTF8, "application/json");
            var resp = await _client.PostAsync($"api/dns/ids", body);
            if (!resp.IsSuccessStatusCode) throw new HttpRequestException(resp.ToString());

            return JsonSerializer.Deserialize<List<DnsEntry>>(await resp.Content.ReadAsStringAsync(),
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> Exist(Guid uid) => await GetEntry(uid) != null;

        public async Task<(List<Uri>, List<C25519Key>)> GetInfo(Guid uid)
        {
            var entry =  (await GetEntries(new Guid[] {uid})).FirstOrDefault();
            if (entry == null) return (new List<Uri>(), new List<C25519Key>());
            
            return (entry.GetUrls(), entry.GetPublics());
        }
    }
}