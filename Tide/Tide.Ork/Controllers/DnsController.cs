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
using Microsoft.Extensions.Logging;
using Tide.Core;
using Tide.Ork.Repo;

namespace Tide.Ork.Controllers
{
    [ApiController]
    [Route("api/dns")]
    public class DnsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDnsManager _manager;

        public DnsController(IKeyManagerFactory factory, ILogger<KeyController> logger)
        {
            _manager = factory.BuildDnsManager();
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DnsEntry>> GetById([FromRoute] Guid id)
        {
            var key = await _manager.GetById(id);
            if (key == null)
                return NotFound();

            return key;
        }

        [HttpPost]
        public async Task<ActionResult> SetOrUpdate([FromBody] DnsEntry entry)
        {
            var result = await _manager.SetOrUpdate(entry);
            if (result.Success) _logger.LogInformation($"DnsEntry added for {entry.Id}");
            else _logger.LogError(result.Error);

            return result.Success ? Ok() : BadRequest(result.Error) as ActionResult;
        }
   }
}