using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
        public IActionResult GetApplication() {
            var jwt = Request.Headers["Authorization"].ToString().Substring(7);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            return Ok(token);
        }

        //[HttpPost]
        //public ActionResult SendApplication([FromBody] RentalApplication application)
        //{
        //    try {
        //        application.DateSubmitted = DateTimeOffset.Now;
        //        User.RentalApplications.Add(application);
        //        Context.Update(User);
        //        Context.SaveChanges();
        //        return Ok();
        //    }
        //    catch (Exception e) {
        //        return BadRequest(e);
        //    }
        //}
    }
}
