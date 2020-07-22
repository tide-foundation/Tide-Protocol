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
using Tide.Ork.Classes;
using Tide.Ork.DTOs;

namespace Tide.Ork.Controllers {
    [ApiController]
    [Route("api/rule")]
    public class RuleController : ControllerBase {
        private readonly ILogger _logger;
        private readonly IRuleManager _manager;

        public RuleController(IKeyManagerFactory factory, ILogger<RuleController> logger) {
            _manager = factory.BuildRuleManager();
            _logger = logger;
        }

        [HttpGet("{ruleId}")]
        public async Task<ActionResult<RuleVaultDTO>> GetById([FromRoute] Guid ruleId)
        {
            var rule = await _manager.GetById(ruleId);
            if (rule == null)
                return NotFound();
            
            return new RuleVaultDTO(rule);
        }

        [HttpGet("user/{vuid}")]
        public async Task<ActionResult<List<RuleVaultDTO>>> GetByUser([FromRoute] Guid vuid)
        {
            return (await _manager.GetSetBy(vuid)).Select(r => new RuleVaultDTO(r)).ToList();
        }

        [HttpPost]
        public async Task<ActionResult> SetOrUpdate([FromBody] RuleVaultDTO rule)
        {
            var result = await _manager.SetOrUpdate(rule);
            return result.Success ? Ok() : BadRequest() as ActionResult;
        }

        [HttpDelete("{ruleId}")]
        public async Task<ActionResult> Delete([FromRoute] Guid ruleId)
        {
            await _manager.Delete(ruleId);
            return Ok();
        }
    }
}