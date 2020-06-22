using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tide.Core;
using Tide.Encryption.Tools;
using Tide.Usecase.Models;

namespace Tide.Usecase.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class VendorController : ControllerBase {

        private const string VENDOR_ID = "VendorId"; // This is temporary auth

        private readonly HttpClient _client;

        public VendorController(Settings settings) {
            _client = new HttpClient { BaseAddress = new Uri(settings.Endpoints.Simulator.Api) };
        }

        [HttpPost("CreateUser/{username}")]
        public ActionResult CreateUser([FromRoute] string username,[FromBody] List<string> desiredOrks) {
            var stringContent = new StringContent(JsonConvert.SerializeObject(desiredOrks), Encoding.UTF8, "application/json");
            var response =  _client.PostAsync($"Simulator/CreateUser/{Helpers.GetTideId(username)}/{VENDOR_ID}", stringContent).Result;

            if (response.IsSuccessStatusCode) return Ok();

            return StatusCode((int) response.StatusCode, response.Content.ReadAsStringAsync().Result);
        }

        [HttpGet("ConfirmUser/{username}")]
        public ActionResult ConfirmUser([FromRoute] string username)
        {
            var response = _client.GetAsync($"Simulator/ConfirmUser/{Helpers.GetTideId(username)}/{VENDOR_ID}").Result;

            if (response.IsSuccessStatusCode) return Ok();
            return StatusCode((int)response.StatusCode, response.Content.ReadAsStringAsync().Result);
        }

    }
}