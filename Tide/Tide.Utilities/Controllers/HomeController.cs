using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tide.Core;
using Tide.Utilities.Models;

namespace Tide.Utilities.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _client = new HttpClient();

        public HomeController()
        {

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dauth()
        {
            return View();
        }

        public async Task<IActionResult> Explorer()
        {
            var response = await _client.GetAsync($"https://localhost:5001/explorer/1/1/Matt/John");
            var json = await response.Content.ReadAsStringAsync();
            var blocks = JsonConvert.DeserializeObject<List<BlockData>>(json);

            return View(blocks);
        }

    }
}
