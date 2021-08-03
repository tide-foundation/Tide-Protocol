using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tide.Core;

namespace Tide.VendorSdk.Classes
{
    public class SimulatorClient
    {
        private static HttpClient _client;
        //private readonly AuthenticationRequest _authRequest; //TODO: Ask matt why is not used.
        private const string _chainEndpoint = "https://tidesimulator.azurewebsites.net/";

        public SimulatorClient(string vendorId)
        {
            _client = new HttpClient { BaseAddress = new Uri(_chainEndpoint) };
            _client.DefaultRequestHeaders.Add("VendorId", vendorId);
        }

        public (bool success,string content) GetUserNodes(string username)
        {
            // if (!IsAuthenticated().Result) return (false, "Authentication failed");

            var response = _client.GetAsync($"Simulator/GetUserNodes/{Helpers.GetTideId(username)}").Result;
            return (response.IsSuccessStatusCode, response.Content.ReadAsStringAsync().Result);
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
        
            var response = _client.GetAsync($"Simulator/ConfirmUser/{Helpers.GetTideId(username)}").Result;

            if (response.IsSuccessStatusCode) return (true, null);
            return (false, response.Content.ReadAsStringAsync().Result);
        }

        public (bool success, string error) RollbackUser(string username)
        {

            var response = _client.GetAsync($"Simulator/RollbackUser/{Helpers.GetTideId(username)}").Result;

            if (response.IsSuccessStatusCode) return (true, null);
            return (false, response.Content.ReadAsStringAsync().Result);
        }

    }
}
