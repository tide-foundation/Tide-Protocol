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
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Tools;
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

        [HttpGet("configuration")]
        public async Task<ActionResult<SignUpRespose>> Configuration()
        {
            return new SignUpRespose
            {
                OrkUrls = await OrkRepo.GetListOrks(),
                PubKey = Config.PrivateKey.GetPublic().ToByteArray()
            };
        }

        [HttpPut("account/{vuid}")]
        public async Task<ActionResult<byte[]>> SignUp([FromRoute] Guid vuid, [FromBody] string auth)
        {
            var authKey = AesKey.Parse(auth);
            await OrkRepo.AddUser(vuid, authKey);

            Logger.LogInformation($"Account created for {vuid}", vuid);
            return TranToken.Generate(Config.SecretKey).ToByteArray(); ;
        }

        [HttpGet("testcipher/{vuid}/{token}/{ciphertext}")]
        public async Task<ActionResult> TestCipher([FromRoute] Guid vuid, [FromRoute] string token, [FromRoute] string ciphertext)
        {
            var tran = TranToken.Parse(FromBase64(token));
            var cipher = FromBase64(ciphertext);
            var plain = await Decript(vuid, cipher);

            if (!tran.Check(Config.SecretKey))
                return BadRequest("Invalid token");

            if (!Utils.Equals(plain, Utils.Hash(tran.ToByteArray())))
                return BadRequest("Invalid decryption");

            Logger.LogInformation($"Successful account creation for {vuid}", vuid);
            return Ok();
        }

        [HttpGet("auth/{vuid}/{token}")]
        public async Task<ActionResult> SignIn([FromRoute] Guid vuid, [FromRoute] string token)
        {
            var tran = TranToken.Parse(FromBase64(token));

            var authKey = await OrkRepo.GetKey(vuid);
            if (authKey == null) return BadRequest("User or invalid signature");

            if (!tran.OnTime || !tran.Check(authKey, vuid.ToByteArray()))
                return BadRequest("User or invalid signature");

            Logger.LogInformation($"successful login for {vuid}", vuid, token);
            return Ok();
        }

        private async Task<byte[]> Decript(Guid vuid, byte[] cipher)
        {
            var uris = (await OrkRepo.GetListOrks()).Select(url => new Uri(url)).ToList();
            var flow = new DCryptFlow(vuid, uris);

            return await flow.Decrypt(cipher, Config.PrivateKey);
        }

        private byte[] FromBase64(string input)
        {
            return Convert.FromBase64String(input.DecodeBase64Url());
        }
   }
}