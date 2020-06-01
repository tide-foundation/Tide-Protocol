using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tide.Simulator.Classes;
using Tide.Simulator.Models;

namespace Tide.Simulator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SimulatorController : ControllerBase
    {
        private readonly IBlockLayer _blockchain;

        public SimulatorController(IBlockLayer blockchain)
        {
            _blockchain = blockchain;
        }

        [HttpGet("GetVault/{ork}/{username}")]
        public ActionResult<string> GetVault(string ork, string username) {
            return _blockchain.Read(Contract.Authentication, Table.Vault, ork, username);
        }

        [HttpPost("PostVault")]
        public ActionResult<bool> PostVault([FromBody] AuthenticationModel model) {
            return _blockchain.Write(Contract.Authentication, Table.Vault, model.Ork, model.User, model.Payload);
        }
    }
}
