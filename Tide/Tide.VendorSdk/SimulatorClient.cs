using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Tide.Core;

namespace Tide.VendorSdk
{
    public class SimulatorClient
    {
        private static HttpClient _client;
        private readonly AuthenticationRequest _authRequest;
        private const string _chainEndpoint = "https://tidesimulator.azurewebsites.net/";

        public SimulatorClient(string vendorId)
        {
            _client = new HttpClient { BaseAddress = new Uri(_chainEndpoint) };
            _client.DefaultRequestHeaders.Add("VendorId", vendorId);
        }

        public (bool success,string error) CreateUser(string username, List<string> desiredOrks)
        {
           // if (!IsAuthenticated().Result) return (false, "Authentication failed");

            var stringContent = new StringContent(JsonConvert.SerializeObject(desiredOrks), Encoding.UTF8, "application/json");
            var response = _client.PostAsync($"Simulator/CreateUser/{Helpers.GetTideId(username)}", stringContent).Result;

            if (response.IsSuccessStatusCode) return (true, null);

            return (false, response.Content.ReadAsStringAsync().Result);
        }

        public (bool success, string error) ConfirmUser(string username)
        {
           // if (!IsAuthenticated().Result) return (false, "Authentication failed");

            var response = _client.GetAsync($"Simulator/ConfirmUser/{Helpers.GetTideId(username)}").Result;

            if (response.IsSuccessStatusCode) return (true, null);
            return (false, response.Content.ReadAsStringAsync().Result);
        }

        public (bool success, string error) RollbackUser(string username)
        {
            // if (!IsAuthenticated().Result) return (false, "Authentication failed");

            var response = _client.GetAsync($"Simulator/RollbackUser/{Helpers.GetTideId(username)}").Result;

            if (response.IsSuccessStatusCode) return (true, null);
            return (false, response.Content.ReadAsStringAsync().Result);
        }


        private async Task<bool> IsAuthenticated()
        {
            if (_client.DefaultRequestHeaders.Authorization == null)
            {
                var authResponse = await GetAuthResponse("Login");
                if (!authResponse.Success)
                {
                    if (authResponse.Error == "Invalid Username") authResponse = await GetAuthResponse("Register");
                    else return false;
                }

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.Token);
            }

            return true;

            async Task<AuthenticationResponse> GetAuthResponse(string path)
            {
                var r = await _client.PostAsync($"Authentication/{path}", new StringContent(JsonConvert.SerializeObject(_authRequest), Encoding.UTF8, "application/json"));
                var j = await r.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<AuthenticationResponse>(j);
            }
        }
    }
}
