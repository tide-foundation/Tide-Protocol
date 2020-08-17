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
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Tools;
using Tide.VendorSdk.Classes;
using Tide.VendorSdk.Models;

namespace Tide.VendorSdk.Controllers {
    [ApiController]
    [Route("tide/vendor")]
    public class VendorController : ControllerBase {
        protected readonly IVendorRepo Repo;
        protected readonly VendorConfig Config;
        protected readonly ILogger Logger;

        public VendorController(IVendorRepo repo, VendorConfig config, ILogger<VendorController> logger) {
            Repo = repo;
            Config = config;
            Logger = logger;
        }

        [HttpGet("configuration")]
        public async Task<ActionResult<ConfRespose>> Configuration()
        {
            return new ConfRespose
            {
                OrkUrls = await Repo.GetListOrks(),
                PubKey = Config.PrivateKey.GetPublic().ToByteArray()
            };
        }

        //TODO: Add throttling ip and port number
        [HttpPut("account/{vuid}")]
        public async Task<ActionResult<SignupRsponse>> SignUp([FromRoute] Guid vuid, [FromBody] SignupRequest data)
        {
            var authKey = AesKey.Parse(data.Auth);
            
            var signatures = data.OrkIds.Select(orkId => orkId.ToByteArray().Concat(vuid.ToByteArray()))
                .Select(msg => Config.PrivateKey.Sign(msg.ToArray())).ToList();
            
            await Repo.CreateUser(vuid, authKey);

            Logger.LogInformation($"Account created for {vuid}", vuid);
            return new SignupRsponse {
                Token = TranToken.Generate(Config.SecretKey, vuid.ToByteArray()).ToByteArray(),
                Signatures = signatures
            };
        }

        [Authorize]
        [HttpGet("testcipher/{vuid}/{token}/{ciphertext}")]
        public async Task<ActionResult> TestCipher([FromRoute] Guid vuid, [FromRoute] string token, [FromRoute] string ciphertext)
        {
            var tran = TranToken.Parse(FromBase64(token));
            var cipher = FromBase64(ciphertext);
            var plain = await Decript(vuid, cipher);

            if (!tran.Check(Config.SecretKey, vuid.ToByteArray())) {
                await Repo.RollbackUser(vuid);
                return BadRequest("Invalid token");
            }

            if (!Utils.Equals(plain, Utils.Hash(tran.ToByteArray()))) {
                await Repo.RollbackUser(vuid);
                return BadRequest("Invalid decryption");
            }

            await Repo.ConfirmUser(vuid);
            Logger.LogInformation($"Successful account creation for {vuid}", vuid);
            return Ok();
        }

        [HttpGet("auth/{vuid}/{token}")]
        public async Task<ActionResult<string>> SignIn([FromRoute] Guid vuid, [FromRoute] string token)
        {
            var tran = TranToken.Parse(FromBase64(token));

            var authKey = await Repo.GetKey(vuid);
            if (authKey == null) return BadRequest("User or invalid signature");

            if (!tran.OnTime || !tran.Check(authKey, vuid.ToByteArray()))
                return BadRequest("User or invalid signature");

            Logger.LogInformation($"successful login for {vuid}", vuid, token);
            return GenerateToken(vuid);
        }

        private async Task<byte[]> Decript(Guid vuid, byte[] cipher)
        {
            var uris = (await Repo.GetListOrks()).Select(url => new Uri(url)).ToList();
            var flow = new DCryptFlow(vuid, uris);

            return await flow.Decrypt(cipher, Config.PrivateKey);
        }

        private byte[] FromBase64(string input)
        {
            return Convert.FromBase64String(input.DecodeBase64Url());
        }

        private string GenerateToken(Guid vuid)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, vuid.ToString())
                }),
                Expires = DateTime.UtcNow.AddYears(10),
                SigningCredentials = new SigningCredentials(Config.GetSessionKey(), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}