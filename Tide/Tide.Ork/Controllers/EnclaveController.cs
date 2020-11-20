using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Tide.Ork.Classes;
using Tide.Ork.Models;

namespace Tide.Ork.Controllers
{
    [ApiController]
    [Route("api/enclave")]
    public class EnclaveController: ControllerBase
    {

        [HttpPost("Deliver")]
        public async Task<IActionResult> Deliver([FromBody]EnclavePackage package) {
            if (EnclaveHub.ClientMapping.TryGetValue(package.SessionId,out var client)) {
                await client.SendAsync("deliver", package.Data);
                return Ok();
            }

            return NotFound();

        }
    }
}
