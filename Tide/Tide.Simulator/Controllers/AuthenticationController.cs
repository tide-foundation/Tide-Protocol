using Tide.Core;
using Microsoft.AspNetCore.Mvc;
using Tide.Simulator.Classes;

namespace Tide.Simulator.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase {
        private readonly IAuthentication _auth;

        public AuthenticationController(IAuthentication auth) {
            _auth = auth;
        }

        [HttpPost]
        public IActionResult Register([FromBody] AuthenticationRequest request) {
            var result = _auth.Register(request);
            if (result.success) return Ok();
            return BadRequest(result.error);
        }
    }
}