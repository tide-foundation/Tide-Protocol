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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tide.Ork.DTOs;
using Tide.Ork.Repo;

namespace Tide.Ork.Controllers {
    [ApiController]
    [Route("api/key")]
    public class KeyController : ControllerBase {
        private readonly ILogger _logger;
        private readonly IKeyIdManager _manager;
        private Guid Username => User.Identity.IsAuthenticated && Guid.TryParse(User.Identity.Name, out var id) ? id : Guid.Empty;

        public KeyController(IKeyManagerFactory factory, ILogger<KeyController> logger) {
            _manager = factory.BuildKeyIdManager();
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KeyIdVaultDTO>> GetById([FromRoute] Guid id)
        {
            var key = await _manager.GetById(id);
            if (key == null) return NotFound();

            return new KeyIdVaultDTO(key);
        }

        [HttpPost]
        public async Task<ActionResult> SetOrUpdate([FromBody] KeyIdVaultDTO key)
        {
            var isRightOwner = User.Identity.IsAuthenticated  && key.KeyId == Username;
            var isNewEntry = false;

            if (isRightOwner || (isNewEntry = !await _manager.Exist(key.KeyId))) {
                var result = await _manager.SetOrUpdate(key);
                if (!result.Success) {
                    _logger.LogError("There is an error with the key repository for {keyId}: {error}", key.KeyId, result.Error);
                    return StatusCode(500);
                }

                _logger.LogInformation("Key {note} for {keyId}", isNewEntry? "added" : "modified", key.KeyId);
                return !isNewEntry ? Ok() : CreatedAtAction(nameof(GetById), new { id = key.KeyId }, key);
            }

            if (!isNewEntry && !User.Identity.IsAuthenticated)
                _logger.LogWarning("An unauthorized user tried to update the key {keyId}", key.KeyId);

            else
                _logger.LogWarning("An unauthorized user {user} tried to update the key {keyId}", Username, key.KeyId);

            return Unauthorized();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            var isRightOwner = User.Identity.IsAuthenticated && id == Username;
            if (!isRightOwner) {
                _logger.LogWarning("An unauthorized user {user} tried to update the key {keyId}", Username, id);
                return Unauthorized();
            }

            await _manager.Delete(id);
            _logger.LogInformation("Key deleted for {keyId}", id);
            return  NoContent();
        }
    }
}