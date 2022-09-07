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
using Tide.Encryption.Ecc;
using Tide.Ork.Controllers;

namespace Tide.Ork.Classes {
    public class SimulatorClient {
        private readonly HttpClient _client;
        private readonly C25519Key _private;
        private readonly string _orkId;
        //private bool _registered; //TODO: Ask matt why is not used

        public SimulatorClient(string url,string orkId, C25519Key privateKey) {
            _private = privateKey;
            _orkId = orkId;
            _client = new HttpClient {BaseAddress = new Uri(url)};
        }

        //private async Task<bool> Authenticated() {
        //    if (_registered) return true;

        //    var stringContent = new StringContent(JsonConvert.SerializeObject(new AuthenticationRequest(_orkId,_private.GetPublic().ToString())), Encoding.UTF8, "application/json");
        //    var response = (await _client.PostAsync("Authentication", stringContent));

        //    _registered = response.IsSuccessStatusCode;
        //    if (!_registered) Console.Write($"Ork was not authorized to write transaction. Error: {await response.Content.ReadAsStringAsync()}");

        //    return _registered;
        //}

        public async Task<(bool success,string error)> Post(string contract, string table, string scope, string index, object payload) {
            //if (!await Authenticated()) return (false,"not authenticated");
            var blockData = new Transaction(contract, table, scope, index, _orkId, payload);

            var serializedPayload  = JsonConvert.SerializeObject(blockData.Data);
            blockData.Sign = _private.EdDSASign(Encoding.UTF8.GetBytes(serializedPayload));

            var stringContent = new StringContent(JsonConvert.SerializeObject(blockData), Encoding.UTF8, "application/json");
            var response = (await _client.PostAsync("Simulator", stringContent));

            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await response.Content.ReadAsStringAsync());
        }

        public async Task<string> Get(string contract, string table, string scope, string index) {
            try {
                return (await FetchTransaction<Transaction>(GeneratePath(contract, table, scope, index))).Data;
            }
            catch (Exception e) {
                Console.Write($"FAILED GATHERING DATA FOR: {GeneratePath(contract, table, scope, index)}. RESPONSE: {e.Message}");
                return null;
            }
        }

        public async Task<List<string>> Get(string contract, string table, string scope, IEnumerable<string> index) {
            try
            {
                var body = new StringContent(JsonConvert.SerializeObject(index), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(GeneratePath(contract, table, scope, null), body);
                var transactions = JsonConvert.DeserializeObject<List<Transaction>>(await response.Content.ReadAsStringAsync());

                return transactions.Select(t => t.Data).ToList();
            }
            catch (Exception e)
            {
                Console.Write($"FAILED GATHERING DATA FOR POST: {GeneratePath(contract, table, scope, null)}. RESPONSE: {e.Message}");
                return null;
            }
        }

        public async Task<List<string>> Get(string contract, string table, string scope)
        {
            try {
                var transactions = await FetchTransaction<List<Transaction>>(GeneratePath(contract, table, scope,null));
                return transactions.Select(t => t.Data).ToList();
            }
            catch (Exception e) {
                Console.Write($"FAILED GATHERING DATA FOR: {GeneratePath(contract, table, scope, null)}. RESPONSE: {e.Message}");
                return null;
            }
        }

        public Task<bool> Delete(string contract, string table, string scope, string index)
        {
           // if (!await Authenticated()) return false;

            //var transaction = await FetchTransaction<Transaction>(GeneratePath(contract, table, scope, index));
            //var sign = Convert.ToBase64String(_private.Sign(Encoding.UTF8.GetBytes(transaction.Id)));

            //using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, GeneratePath(contract, table, scope, index))) {
            //    requestMessage.Headers.Add("sign", sign);
            //    var result = await _client.SendAsync(requestMessage);
            //    return result.IsSuccessStatusCode;
            //}
            return Task.FromResult(false);
        }

        private string GeneratePath(string contract, string table, string scope, string index) {
            return string.Join("/", new[] { "Simulator", contract, table, scope, index }
                .Where(itm => !string.IsNullOrWhiteSpace(itm)));
        }

        private async Task<T> FetchTransaction<T>(string location) {
            var response = await _client.GetAsync(location);
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }
    }
}
