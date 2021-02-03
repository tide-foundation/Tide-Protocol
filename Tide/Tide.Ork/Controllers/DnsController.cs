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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tide.Core;
using Tide.Ork.Classes;
using Tide.Ork.Models;
using Tide.Ork.Repo;

namespace Tide.Ork.Controllers
{
    [ApiController]
    [Route("api/dns")]
    public class DnsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDnsManager _manager;
        private readonly SimulatorOrkManager _orkManager;
        private readonly string _orkId;

        public DnsController(IKeyManagerFactory factory, ILogger<KeyController> logger, Settings settings)
        {
            _manager = factory.BuildDnsManager();
            _logger = logger;
            _orkId = settings.Instance.Username;

            var cln = new SimulatorClient(settings.Endpoints.Simulator.Api, _orkId, settings.Instance.GetPrivateKey());
            _orkManager = new SimulatorOrkManager(_orkId, cln);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DnsEntry>> GetById([FromRoute] Guid id)
        {
            var entry = (await GetByIds(new[] { id })).FirstOrDefault();
            if (entry == null) return NotFound();
            
            return entry;
        }

        [HttpPost("ids")]
        public async Task<List<DnsEntry>> GetByIds([FromBody] Guid[] ids)
        {
            var orksInfoTask = await _orkManager.GetAll();
            var entries = await _manager.GetByIds(ids);

            foreach (var entry in entries)
            {
                var infOrks = (from orkId in entry.Orks
                               join info in (orksInfoTask) on orkId equals info.Id into inf
                               from defInf in inf.DefaultIfEmpty()
                               select defInf).ToArray();

                entry.Urls = infOrks.Select(inf => inf?.Url ?? string.Empty).ToArray();
                entry.Publics = infOrks.Select(inf => inf?.PubKey ?? string.Empty).ToArray();
            }

            return entries;
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