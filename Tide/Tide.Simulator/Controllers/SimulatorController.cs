using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tide.Core;
using Tide.Simulator.Classes;
using Tide.Simulator.Models;

namespace Tide.Simulator.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class SimulatorController : ControllerBase
    {
        private readonly IBlockLayer _blockchain;

        private static readonly object WriteLock = new object();

        public SimulatorController(IBlockLayer blockchain)
        {
            _blockchain = blockchain;


        }

        #region Onboarding

        // VENDOR CALL
        [HttpPost("CreateUser/{username}/{vendorId}")]
        public ActionResult CreateUser([FromRoute] string username, string vendorId, [FromBody] List<string> desiredOrks)
        {
            if (GetUser(username, out var user)) return Conflict("That username already exists");

            var newUser = new User
            {
                Vendor = vendorId
            };

            foreach (var desiredOrk in desiredOrks)
            {
                newUser.Nodes.Add(new OrkStatus() { Ork = desiredOrk });
            }

            return SetUser(username, newUser) ? Ok() : StatusCode(500);
        }

        // VENDOR CALL
        [HttpGet("ConfirmUser/{username}/{vendorId}")]
        public ActionResult ConfirmUser([FromRoute] string username, string vendorId)
        {
            if (!GetUser(username, out var user)) return BadRequest("That username does not exist");

            if (user.Vendor != vendorId) return BadRequest("You are not the authorized vendor");
            if (user.Status != UserStatus.Pending) return BadRequest("That user is not in the pending state");

            if (!user.Nodes.All(n => n.Confirmed)) return BadRequest("All ork nodes have not been confirmed");

            user.Status = UserStatus.Active;

            return SetUser(username, user) ? Ok() : StatusCode(500);
        }

        // VENDOR CALL
        [HttpDelete("RollbackUser/{username}")]
        public ActionResult RollbackUser([FromRoute] string username)
        {
            if (!GetUser(username, out var user)) return BadRequest("That username does not exist");

            if (user.Status != UserStatus.Pending) return BadRequest("Only pending users can be rolled back");

            return _blockchain.SetStale(Contract.Authentication, Table.Users, Contract.Authentication.ToString(), username) ? Ok() : StatusCode(500);
        }

        [HttpGet("Vault/{ork}/{username}")]
        public ActionResult GetVault([FromRoute] string ork, string username)
        {
            var result = _blockchain.Read(Contract.Authentication, Table.Vault, ork, username);
            if (string.IsNullOrEmpty(result)) return BadRequest();
            return Ok(result);
        }

        //[Authorize]
        // ORK CALL
        [HttpPost("Vault/{ork}/{username}")]
        public TideResponse PostVault([FromRoute] string ork, string username, [FromBody] string payload)
        {
            lock (WriteLock)
            {
                if (!GetUser(username, out var user)) return new TideResponse("That username does not exist");

                var desiredOrk = user.Nodes.FirstOrDefault(o => o.Ork == ork);
                if (desiredOrk == null) return new TideResponse("You are not a desired Ork node. Ork: " + ork);

                desiredOrk.Confirmed = true;

                var transactions = new List<BlockData> {
                    new BlockData(Contract.Authentication, Table.Vault, ork, username, payload),
                    new BlockData(Contract.Authentication, Table.Users, Contract.Authentication.ToString(), username, JsonConvert.SerializeObject(user))
                };

                //if (HttpContext.User.Identity.Name != ork) return Unauthorized("You do not have write privileges to that scope.");
                return _blockchain.Write(transactions) ? new TideResponse() : new TideResponse("Internal Server Error");
            }
        }

        #endregion


        #region Helpers

        private bool GetUser(string username, out User user)
        {
            user = null;
            var data = _blockchain.Read(Contract.Authentication, Table.Users, Contract.Authentication.ToString(), username);

            if (data == null) return false;

            user = JsonConvert.DeserializeObject<User>(data);
            return true;
        }

        private bool SetUser(string username, User user)
        {
            return _blockchain.Write(new BlockData(Contract.Authentication, Table.Users, Contract.Authentication.ToString(), username, JsonConvert.SerializeObject(user)));
        }

        #endregion
    }
}
