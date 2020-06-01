using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Tide.Simulator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SimulatorController : ControllerBase
    {
        //private readonly IContractLayer _contract;

        //public SimulatorController(IContractLayer contract)
        //{
        //    _contract = contract;
        //}

        //[HttpGet("GetUser/{username}")]
        //public ActionResult<SimulatorUser> GetUser(string username)
        //{
        //    return _contract.GetUser(username);
        //}

        //// GET api/values/5
        //[HttpGet("{id}")]
        //public ActionResult<string> Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //[HttpGet("CreateUser/{username}")]
        //public SimulatorUser CreateUser(string username)
        //{
        //    return _contract.CreateUser(username, "password");
        //}

        ////[HttpGet("AddFrag")]
        ////public bool AddFrag(AuthenticationModel model)
        ////{
        ////    return _contract.AddFrag(username, ork);
        ////}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
