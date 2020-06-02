using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Tide.Core;
using Newtonsoft.Json;

namespace Tide.Ork.Classes {
    public class SimulatorClient {
        private readonly AuthenticationRequest _authRequest;
        private readonly HttpClient _client;

        public SimulatorClient(string url, string orkId, string password) {
            _authRequest = new AuthenticationRequest(orkId, password);
            _client = new HttpClient {BaseAddress = new Uri(url)};
        }

        public async Task PostVault(string ork, string username, string payload) {
            if (!await IsAuthenticated()) return;

            var stringContent = new StringContent($"\"{payload}\"", Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"Simulator/Vault/{ork}/{username}", stringContent);
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> GetVault(string ork, string username) {
            var response = await _client.GetAsync($"Simulator/Vault/{ork}/{username}");
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<bool> IsAuthenticated() {
            if (_client.DefaultRequestHeaders.Authorization == null) {
                var authResponse = await GetAuthResponse("Login");
                if (!authResponse.Success) {
                    if (authResponse.Error == "Invalid Username") authResponse = await GetAuthResponse("Register");
                    else return false;
                }

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.Token);
            }

            return true;

            async Task<AuthenticationResponse> GetAuthResponse(string path) {
                var r = await _client.PostAsync($"Authentication/{path}", new StringContent(JsonConvert.SerializeObject(_authRequest), Encoding.UTF8, "application/json"));
                var j = await r.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AuthenticationResponse>(j);
            }
        }
    }
}