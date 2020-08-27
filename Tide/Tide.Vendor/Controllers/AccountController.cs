using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tide.Vendor.Models;
using Tide.VendorSdk.Classes;

namespace Tide.Vendor.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly VendorDbContext _context;

        public AccountController(VendorDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public ActionResult<object> CreateAccount([FromBody] ApplicationUser user) {
            return _context.CreateApplicationUser(user);
        }

    }
}
