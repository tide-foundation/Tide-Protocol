using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Tide.Core;
using Newtonsoft.Json;

namespace Tide.Ork.Classes {
    public class SimulatorClient {
        private readonly HttpClient _client;

        public SimulatorClient(string url, string orkId, string password) {
            _client = new HttpClient {BaseAddress = new Uri(url)};
        }

        public async Task<bool> Post(string contract, string table, string scope, string index, object payload) {
            var blockData = new BlockData(contract, table, scope, index, JsonConvert.SerializeObject(payload));
            var stringContent = new StringContent(JsonConvert.SerializeObject(blockData), Encoding.UTF8, "application/json");
            var response = (await _client.PostAsync("Simulator", stringContent));
            
            return response.IsSuccessStatusCode;
        }

        public async Task<T> Get<T>(string contract, string table, string scope, string index)
        {
            var response = await _client.GetAsync($"Simulator/{contract}/{table}/{scope}{(string.IsNullOrEmpty(index) ? "" : $"/{index}")}");
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }
    }
}