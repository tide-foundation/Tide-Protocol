using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tide.Core;
using Tide.Ork.Classes;
using Tide.Ork.Models;

namespace Tide.Ork.Controllers
{
    [ApiController]
    [Route("api/discovery")]
    public class DiscoveryController : ControllerBase
    {
        private readonly Settings _settings;
        private readonly SimulatorClient _client;

        public DiscoveryController(Settings settings,SimulatorClient client) {
            _settings = settings;
            _client = client;
        }

        [HttpGet("/discover")]
        public  Core.OrkNode Discover() {
            return new OrkNode() {
                Id = _settings.Instance.Username, Url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}",
                Website = "https://tide.org/"
            };
        }

        [HttpGet("/GetOrks/{amount}")]
        public TideResponse GetOrks([FromRoute] int amount) {

            var list = new List<OrkNode>();
            for (int i = 0; i < amount; i++) {
                list.Add(new OrkNode(){Id = i.ToString(),Url = $"https://ork-${i}.azurewebsites.net/",Website = "https://tide.org/" });
            }

            return new TideResponse(true,JsonConvert.SerializeObject(list),null);

            return null;
            //var response = _client.GetData($"Simulator/getorks").Result;
            //var stringList = JsonConvert.DeserializeObject<List<string>>((string)response.Content) ;
            //response.Content = stringList.Select(JsonConvert.DeserializeObject<Core.OrkNode>).Take(amount);
            //return response;
        }
    }
}
