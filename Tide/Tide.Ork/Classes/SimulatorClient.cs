using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tide.Ork.Classes
{
    public class SimulatorClient
    {
        private readonly HttpClient _client;

        public SimulatorClient(string endpoints) {
            _client = new HttpClient { BaseAddress = new Uri(endpoints) };
        }

        public async Task PostVault(string ork, string username, string payload) {
            var stringContent = new StringContent($"\"{payload}\"",Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"Vault/{ork}/{username}", stringContent);
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> GetVault(string ork, string username)
        {
            var response = await _client.GetAsync($"Vault/{ork}/{username}");
            return await response.Content.ReadAsStringAsync();
        }
    }
}
