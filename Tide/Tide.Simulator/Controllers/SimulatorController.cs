using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tide.Simulator.Classes;
using Tide.Simulator.Models;

namespace Tide.Simulator.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class SimulatorController : ControllerBase {
        private readonly IBlockLayer _blockchain;

        public SimulatorController(IBlockLayer blockchain) {
            _blockchain = blockchain;
        }

        [HttpGet("Vault/{ork}/{username}")]
        public ActionResult<string> GetVault([FromRoute] string ork, string username) {
            return _blockchain.Read(Contract.Authentication, Table.Vault, ork, username);
        }

        [Authorize]
        [HttpPost("Vault/{ork}/{username}")]
        public ActionResult<bool> PostVault([FromRoute] string ork, string username, [FromBody] string payload) {
            if (HttpContext.User.Identity.Name != ork) return false;

            return _blockchain.Write(Contract.Authentication, Table.Vault, ork, username, payload);
        }
    }
}
