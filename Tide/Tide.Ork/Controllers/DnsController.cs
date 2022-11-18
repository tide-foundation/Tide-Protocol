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
using System.Text.Json;
using Tide.Encryption.Ed;

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
        private Guid Username => User.Identity.IsAuthenticated && Guid.TryParse(User.Identity.Name, out var id) ? id : Guid.Empty;

        public DnsController(IKeyManagerFactory factory, ILogger<DnsController> logger, Settings settings)
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
            var reqTask = _orkManager.GetAll();
            var entriesTask = _manager.GetByIds(ids);
            await Task.WhenAll(reqTask, entriesTask);

            var orksInfoTask = await reqTask;
            var entries = await entriesTask;

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
            var isRightOwner = User.Identity.IsAuthenticated && entry.Id == Username;
            var isNewEntry = false;

            if (isRightOwner || (isNewEntry = !await _manager.Exist(entry.Id)))
            {
                var result = await _manager.SetOrUpdate(entry);
                if (!result.Success)
                {
                    _logger.LogError("There is an error with the dns repository for {dnsId}: {error}", entry.Id, result.Error);
                    return StatusCode(500);
                }

                _logger.LogInformation("DnsEntry {note} for {dnsId}", isNewEntry ? "added" : "modified", entry.Id);
                return !isNewEntry ? Ok() : CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
            }

            if (!isNewEntry && !User.Identity.IsAuthenticated)
                _logger.LogWarning("An unauthorized user tried to update the dns {dnsId}", entry.Id);

            else
                _logger.LogWarning("An unauthorized user {user} tried to update the dns {dnsId}", Username, entry.Id);

            return Unauthorized();
        }

        [HttpGet("ork/{id}")]
        public async Task<ActionResult<string>> GetByOrkId([FromRoute] string id)
        {
            var orkNode =_orkManager.GetById(id);
            var orkInfoTask = await orkNode;
            return orkInfoTask.PubKey;
        }

        [HttpGet("orks/public")]
        public async Task<ActionResult<string[]>> GetPubOrksByIds([FromQuery] string[] orkIds)
        {
            string[] resp = new string [orkIds.Count()];
            for (int i=0 ; i <orkIds.Count(); i++ )
            {
                var orkNode =_orkManager.GetById(orkIds[i]);
                var orkInfoTask = await orkNode;
                var response = new {
                    orkId= orkIds[i] ,
                    pub = Ed25519Key.ParsePublic(orkInfoTask.PubKey)
                };
                resp[i] = JsonSerializer.Serialize(response);
            }
            return resp ;
        }
    }
}