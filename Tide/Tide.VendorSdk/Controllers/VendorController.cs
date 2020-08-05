// Tide Protocol - Infrastructure for the Personal Data economy
// Copyright (C) 2019 Tide Foundation Ltd
// 
// This program is free software and is subject to the terms of 
// the Tide Community Open Source License as published by the 
// Tide Foundation Limited. You may modify it and redistribute 
// it in accordance with and subject to the terms of that License.
// This program is distributed WITHOUT WARRANTY of any kind, 
// including without any implied warranty of MERCHANTABILITY or 
// FITNESS FOR A PARTICULAR PURPOSE.
// See the Tide Community Open Source License for more details.
// You should have received a copy of the Tide Community Open 
// Source License along with this program.
// If not, see https://tide.org/licenses_tcosl-1-0-en

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tide.VendorSdk.Classes;
using Tide.VendorSdk.Models;

namespace Tide.VendorSdk.Controllers {
    public abstract class VendorController : ControllerBase {
        protected readonly IOrkRepo OrkRepo;
        protected readonly VendorConfig Config;
        protected readonly ILogger Logger;

        public VendorController(IOrkRepo repo, VendorConfig config, ILogger<VendorController> logger) {
            OrkRepo = repo;
            Config = config;
            Logger = logger;
        }

        [HttpGet("register")]
        public async Task<ActionResult<SignUpRespose>> SignUp()
        {
            return new SignUpRespose
            {
                OrkUrls = await OrkRepo.GetListOrks(),
                PubKey = Config.PrivateKey.GetPublic().ToByteArray()
            };
        }

        [HttpPost("cipher/vuid/{vuid}")]
        public async Task<ActionResult<string>> TestCipher([FromRoute] Guid vuid, [FromBody] string cipherText)
        {
            var uris = (await OrkRepo.GetListOrks()).Select(url => new Uri(url)).ToList();
            var flow = new DCryptFlow(vuid, uris);

            var cipher = Convert.FromBase64String(cipherText);
            var plain = await flow.Decrypt(cipher, Config.PrivateKey);

            return Convert.ToBase64String(plain);
        }
    }
}