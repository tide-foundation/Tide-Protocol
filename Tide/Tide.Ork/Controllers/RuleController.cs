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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tide.Ork.Classes.Rules;
using Tide.Ork.DTOs;
using Tide.Ork.Repo;

namespace Tide.Ork.Controllers {
    [ApiController]
    [Route("api/rule")]
    public class RuleController : ControllerBase {
        private readonly ILogger _logger;
        private readonly IRuleManager _manager;
        private Guid Username => User.Identity.IsAuthenticated && Guid.TryParse(User.Identity.Name, out var id) ? id : Guid.Empty;

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
            try { new RuleConditionEval(rule).Eval().Compile(); }
            catch {
                _logger.LogError($"Invalid rule condition for user {rule.OwnerId}: {rule.Condition}");
                return BadRequest("Invalid rule condition");
            }

            var isRightOwner = User.Identity.IsAuthenticated && rule.OwnerId == Username;
            var isNewEntry = false;

            if (isRightOwner || (isNewEntry = !await _manager.Exist(rule.RuleId)))
            {
                var result = await _manager.SetOrUpdate(rule);
                if (!result.Success)
                {
                    _logger.LogError("There is an error with the rule repository for {ruleId}: {error}", rule.RuleId, result.Error);
                    return StatusCode(500);
                }

                _logger.LogInformation($"Rule added from user {rule.OwnerId} with tag {rule.Tag} for key {rule.KeyId}");
                return !isNewEntry ? Ok() : CreatedAtAction(nameof(GetById), new { rule.RuleId }, rule);
            }

            if (!isNewEntry && !User.Identity.IsAuthenticated)
                _logger.LogWarning("An unauthorized user tried to update the rule {ruleId} owned by {id}", rule.RuleId, rule.OwnerId);

            else
                _logger.LogWarning("An unauthorized user {user} tried to update the rule {ruleId} owned by {id}", Username, rule.RuleId, rule.OwnerId);

            return Unauthorized();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            var rule = await _manager.GetById(id);
            if (rule == null) {
                _logger.LogWarning("The user {user} tried to delete the rule {ruleId} that does not existed", Username, id);
                return Unauthorized();
            }

            var isRightOwner = User.Identity.IsAuthenticated && rule.OwnerId == Username;
            if (!isRightOwner)
            {
                _logger.LogWarning("An unauthorized user {user} tried to delete the rule {ruleId} owned by {id}", Username, id, rule.OwnerId);
                return Unauthorized();
            }

            await _manager.Delete(id);
            _logger.LogInformation("Rule deleted for {ruleId}", id);
            return NoContent();
        }
    }
}