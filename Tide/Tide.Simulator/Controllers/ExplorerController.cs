using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tide.Core;
using Tide.Simulator.Classes;
using Tide.Simulator.Models;

namespace Tide.Simulator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExplorerController : ControllerBase
    {

        private readonly IBlockLayer _blockchain;

        public ExplorerController(IBlockLayer blockchain)
        {
            _blockchain = blockchain;
        }

        [HttpGet("{contract}/{table}/{scope}/{index}")]
        public ActionResult<List<BlockData>> GetHistoric([FromRoute] string contract, string table, string scope, string index)
        {
            return _blockchain.ReadHistoric(contract, table, scope, index);
        }
    }
}