using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Numerics;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tide.Core;
using Tide.Encryption.Tools;
using Tide.Usecase.Models;
using Tide.VendorSdk;

namespace Tide.Usecase.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class VendorController : ControllerBase {
        private readonly TideVendor _tideVendor;

        public VendorController(TideVendor tideVendor) {
            _tideVendor = tideVendor;
        }

        [HttpPost("CreateUser/{username}")]
        public TideResponse CreateUser([FromRoute] string username,[FromBody] List<string> desiredOrks) {
            return _tideVendor.CreateUser(username, desiredOrks);
        }

        [HttpGet("ConfirmUser/{username}")]
        public TideResponse ConfirmUser([FromRoute] string username)
        {
            return _tideVendor.ConfirmUser(username);
        }
    }
}