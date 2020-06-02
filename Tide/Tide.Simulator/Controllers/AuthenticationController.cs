using Library;
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

        [HttpPost("Register")]
        public AuthenticationResponse Register([FromBody] AuthenticationRequest request) {
            return _auth.Register(request);
        }

        [HttpPost("Login")]
        public AuthenticationResponse Login([FromBody] AuthenticationRequest request) {
            return _auth.Login(request);
        }
    }
}