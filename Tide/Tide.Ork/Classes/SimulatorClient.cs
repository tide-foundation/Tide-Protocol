using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Tide.Core;
using Newtonsoft.Json;
using Tide.Ork.Controllers;

namespace Tide.Ork.Classes {
    public class SimulatorClient {
        private readonly HttpClient _client;

        public SimulatorClient(string url, string orkId, string password) {
            _client = new HttpClient {BaseAddress = new Uri(url)};
        }

        public async Task<bool> Post(string contract, string table, string scope, string index, object payload) {
            var blockData = new Transaction(contract, table, scope, index, payload);
            var stringContent = new StringContent(JsonConvert.SerializeObject(blockData), Encoding.UTF8, "application/json");
            var response = (await _client.PostAsync("Simulator", stringContent));
            
            return response.IsSuccessStatusCode;
        }

        public async Task<string> Get(string contract, string table, string scope, string index) {
            HttpResponseMessage response = null;
            try {
                response = await _client.GetAsync(GeneratePath(contract, table, scope, index));
                var transaction = JsonConvert.DeserializeObject<Transaction>(await response.Content.ReadAsStringAsync());
                return transaction.Data.ToString();
            }
            catch (Exception) {
                
                Console.Write($"FAILED GATHERING DATA FOR: {GeneratePath(contract, table, scope, index)}. RESPONSE: {response.Content.ReadAsStringAsync().Result}");
                return null;
            }
          
        }

        public async Task<List<string>> Get(string contract, string table, string scope)
        {
            try {
                var response = await _client.GetAsync(GeneratePath(contract, table, scope, null));
                var transaction = JsonConvert.DeserializeObject<List<Transaction>>(await response.Content.ReadAsStringAsync());
                return transaction.Select(t => t.Data.ToString()).ToList();
            }
            catch (Exception) {
                Console.Write($"FAILED GATHERING DATA FOR: {GeneratePath(contract, table, scope, null)}");
                return null;
            }
          
        }

        public async Task<bool> Delete(string contract, string table, string scope, string index)
        {
            return (await _client.DeleteAsync(GeneratePath(contract,table,scope,index))).IsSuccessStatusCode;
        }

        private string GeneratePath(string contract, string table, string scope, string index) {
            return $"Simulator/{contract}/{table}/{scope}{(string.IsNullOrEmpty(index) ? "" : $"/{index}")}";
        }
    }
}
