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
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthentication _auth;

        public AuthenticationController(IAuthentication auth)
        {
            _auth = auth;
        }

        [AllowAnonymous]
        [HttpPost("Exchange")]
        public IActionResult Exchange([FromBody] AuthRequest request)
        {
            var resp = _auth.Exchange(request.TideToken, request.Vuid);
            if (resp.success) return Ok(resp.content);
            return Unauthorized();
        }

        [HttpGet]
        public bool Test() {
            return true;
        }


    }

    public class AuthRequest {
        public string Vuid { get; set; }
        public string TideToken { get; set; }
    }

    public class AuthResponse {
        public string Token { get; set; }
    }
}
