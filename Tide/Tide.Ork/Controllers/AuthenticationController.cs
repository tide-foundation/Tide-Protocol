﻿using Microsoft.AspNetCore.Mvc;
using Tide.Library.Models;
using Tide.Library.Models.Interfaces;
using Tide.Ork.Classes;

namespace Tide.Ork.Controllers {
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase {
        private readonly IOrkAuthentication _orkAuthentication;

        public AuthenticationController(OrkAuthentication orkAuthentication) {
            _orkAuthentication = orkAuthentication;
        }

        public IActionResult GetNodes(AuthenticationModel model) {
            return new JsonResult(_orkAuthentication.GetNodes(model));
        }

        public IActionResult PostFragment(AuthenticationModel model) {
            return new JsonResult(_orkAuthentication.PostFragment(model));
        }

        public IActionResult GetFragment(AuthenticationModel model) {
            return new JsonResult(_orkAuthentication.GetFragment(model));
        }
    }
}