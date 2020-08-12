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
using Tide.Ork.DTOs;
using Tide.Ork.Repo;

namespace Tide.Ork.Controllers {
    [ApiController]
    [Route("api/key")]
    public class KeyController : ControllerBase {
        private readonly ILogger _logger;
        private readonly IKeyIdManager _manager;

        public KeyController(IKeyManagerFactory factory, ILogger<KeyController> logger) {
            _manager = factory.BuildKeyIdManager();
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<KeyIdVaultDTO>> GetById([FromRoute] Guid id)
        {
            var key = await _manager.GetById(id);
            if (key == null)
                return NotFound();

            return new KeyIdVaultDTO(key);
        }

        [HttpPost]
        public async Task<ActionResult> SetOrUpdate([FromBody] KeyIdVaultDTO key)
        {
            var result = await _manager.SetOrUpdate(key);
            return result.Success ? Ok() : BadRequest() as ActionResult;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            await _manager.Delete(id);
            return Ok();
        }
    }
}