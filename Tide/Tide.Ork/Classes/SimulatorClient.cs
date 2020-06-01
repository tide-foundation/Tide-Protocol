using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tide.Ork.Models;

namespace Tide.Ork.Classes
{
    public class SimulatorClient
    {
        private readonly HttpClient _client;

        public SimulatorClient(Settings settings) {
            _client = new HttpClient {BaseAddress = new Uri(settings.Endpoints.Simulator)};
        }

        public async Task<(bool successful, string error)> PostVault(string ork, string username, string payload) {
            try {
                var stringContent = new StringContent($"\"{payload}\"",Encoding.UTF8, "application/json");
                var response = await _client.PostAsync($"Vault/{ork}/{username}", stringContent);
                response.EnsureSuccessStatusCode();
                return (true,null);
            }
            catch (Exception e) {
                return (false,e.Message);
            }
        }

        public async Task<(bool successful, string error)> GetVault(string ork, string username)
        {
            try
            {
                var response = await _client.GetAsync($"Vault/{ork}/{username}");
                var data = await response.Content.ReadAsStringAsync();
               return (true, data);
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }
    }
}
