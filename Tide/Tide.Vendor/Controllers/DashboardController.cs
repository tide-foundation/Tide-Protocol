using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tide.Vendor.Classes;

namespace Tide.Vendor.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class DashboardController : ControllerBase
    {

        public DashboardController()
        {
         
        }


        [HttpGet]
        public IActionResult Dashboard() {
            return Ok("This is protected data");
        }
    }
}
