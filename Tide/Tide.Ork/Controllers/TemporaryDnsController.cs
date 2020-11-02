using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tide.Ork.Classes;
using Tide.Ork.Models;

namespace Tide.Ork.Controllers {
    [ApiController]
    [Route("api/TemporaryDns")]
    public class TemporaryDnsController : ControllerBase {
        private const string Contract = "Dns";
        private const string Table = "Accounts";
        private const string UserOrkScope = "User_Orks";
        private const string GlobalOrkScope = "Global_Orks";
        private readonly SimulatorClient _client;

        public TemporaryDnsController(Settings settings) {
            _client = new SimulatorClient(settings.Endpoints.Simulator.Api, settings.Instance.Username, settings.Instance.GetPrivateKey());
        }


        [HttpGet("UserExists/{uid}")]
        public async Task<IActionResult> DoesUserExist([FromRoute] string uid) {
            return Ok(await DoesUserExistCall(uid));
        }

        [HttpPost("UserOrks/{uid}")]
        public async Task<IActionResult> SetUserOrks([FromRoute] string uid, [FromBody] List<string> orks) {
            if (await DoesUserExistCall(uid)) return Conflict("That username already exists");
            var result = await _client.Post(Contract, Table, UserOrkScope, uid, orks);
            if (result.success) return Ok();
            return BadRequest(result.error);
        }

        [HttpGet("UserOrks/{uid}")]
        public async Task<IActionResult> GetUserOrks([FromRoute] string uid) {
            var result = await _client.Get(Contract, Table, UserOrkScope, uid);
            if (result != null) return Ok(result);
            return NotFound("That username does not exist");
        }

        [HttpGet("GlobalOrks/{mode}")]
        public async Task<IActionResult> GetGlobalOrks([FromRoute] string mode)
        {
            var result = await _client.Get(Contract, Table, GlobalOrkScope, mode);
            if (result != null) return Ok(result);
            return NotFound("That mode is not supported");
        }

        [HttpPost("GlobalOrks/{mode}")]
        public async Task<IActionResult> SetGlobalOrks([FromRoute] string mode,[FromBody] List<string> orks)
        {
            var result = await _client.Post(Contract, Table, GlobalOrkScope, mode, orks);
            if (result.success) return Ok();
            return BadRequest(result.error);
        }

        private async Task<bool> DoesUserExistCall(string uid) {
            return await _client.Get(Contract, Table, UserOrkScope, uid) != null;
        }
    }
}