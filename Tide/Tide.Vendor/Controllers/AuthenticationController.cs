using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tide.Core;
using Tide.Vendor.Classes;

namespace Tide.Vendor.Controllers
{
   
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthentication _auth;
        public static Dictionary<string, VendorUser> Users = new Dictionary<string, VendorUser>();

        public AuthenticationController(IAuthentication auth)
        {
            _auth = auth;
        }

        
        [HttpPost("Register")]
        public IActionResult Register([FromBody] AuthRequest request)
        {
            return Ok(_auth.Register(request.TideToken, request.PublicKey));
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] AuthRequest request)
        {
            return Ok(_auth.Login(request.TideToken, request.Vuid));
        }

       
        [HttpGet]
        public IActionResult Get()
        {
            var user = _auth.GetUser(Request.Headers["Authorization"].ToString().Substring(7));
            return Ok(user.Vuid);
        }

      
        [HttpGet("serverTime")]
        public IActionResult GetServerTime() {
            return Ok(DateTime.Now.AddMinutes(1).Ticks);
        }

    }

    public class AuthRequest {
        public string Vuid { get; set; }
        public string PublicKey { get; set; }
        public string TideToken { get; set; }
    }

    public class AuthResponse {
        public string Token { get; set; }
    }

    public class VendorUser {
        public string Vuid { get; set; }
        public string PublicKey { get; set; }
    }
}
