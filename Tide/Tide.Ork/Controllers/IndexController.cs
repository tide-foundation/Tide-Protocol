// Tide Protocol - Infrastructure for the Personal Data economy
// Copyright (C) 2019 Tide Foundation Ltd
// 
// This program is free software and is subject to the terms of 
// the Tide Community Open Source License as published by the 
// Tide Foundation Limited. You may modify it and redistribute 
// it in accordance with and subject to the terms of that License.
// This program is distributed WITHOUT WARRANTY of any kind, 
// including without any implied warranty of MERCHANTABILITY or 
// FITNESS FOR A PARTICULAR PURPOSE.
// See the Tide Community Open Source License for more details.
// You should have received a copy of the Tide Community Open 
// Source License along with this program.
// If not, see https://tide.org/licenses_tcosl-1-0-en

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Tide.Core;
using Tide.Ork.Classes;
using Tide.Ork.Models;
using Tide.Ork.Repo;

namespace Tide.Ork.Controllers
{
    [ApiController]
    [Route("api")]
    public class IndexController : ControllerBase
    {
        private readonly OrkConfig _config;
        private readonly SimulatorOrkManager _orkManager;
        
        public IndexController(OrkConfig config, Settings settings)
        {
            _config = config;
            _orkManager = new SimulatorOrkManager(config.UserName, settings.BuildClient());
        }
 
       [HttpGet("public")]
        public ActionResult<string> GetPublic() {
            Response.Headers[HeaderNames.CacheControl] = "public, max-age=1800, immutable";
            Response.Headers[HeaderNames.Expires] = new[] { DateTime.UtcNow.AddSeconds(1800).ToString("R") };
            
            return _config.PrivateKey.GetPublic().ToString();
        }

        #if DEBUG
        [HttpGet("register")]
        public async Task Register()
        {
            await _orkManager.Add(new OrkNode {
                Id = _config.UserName,
                Url = $"{Request.Scheme}://{Request.Host}",
                PubKey = _config.PrivateKey.GetPublic().ToString()
            });
        }
        #endif
    }
}