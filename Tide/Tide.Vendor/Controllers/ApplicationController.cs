using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tide.Core;
using Tide.Vendor.Models;

namespace Tide.Vendor.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ApplicationController : BaseController
    {

        public ApplicationController(VendorDbContext context) : base(context) {

        }

        [HttpGet]
        public TideResponse GetApplication() {
            var application = Context.Applications.FirstOrDefault(a => a.UserId == User.Id);
            if (application == null) return new TideResponse(false,null,"Not Found");

            return new TideResponse(true,application,null);
        }


        [HttpPost]
        public ActionResult SendApplication([FromBody] RentalApplication application)
        {
            try {
                application.DateSubmitted = DateTimeOffset.Now;
                application.UserId = User.Id;
                Context.Add(application);
                Context.SaveChanges();
                return Ok();
            }
            catch (Exception e) {
                return BadRequest(e);
            }
        
        }

    }
}
