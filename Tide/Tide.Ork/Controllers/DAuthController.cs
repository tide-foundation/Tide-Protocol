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
using System.Numerics;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ecc;
using Tide.Encryption.Tools;
using Tide.Ork.Classes;

namespace Tide.Ork.Controllers {
    [ApiController]
    [Route("api/dauth")]
    public class DAuthController : ControllerBase {
        private readonly IEmailClient _mail;
        private readonly ILogger _logger;
        private readonly IdGenerator _generator;
        private readonly IKeyManager _manager;

        public DAuthController(IKeyManagerFactory factory, IEmailClient mail, ILogger<DAuthController> logger, IdGenerator gen) {
            _manager = factory.BuildManager();
            _mail = mail;
            _logger = logger;
            _generator = gen;
        }

        [HttpGet("{user}/share/{pass}")]
        public async Task<ActionResult> GetShare([FromRoute] string user, [FromRoute] string pass) {
            var g = C25519Point.From(Convert.FromBase64String(pass.DecodeBase64Url()));
            if (!g.IsValid) return BadRequest();

            var s = await _manager.GetAuthShare(GetUserId(user));
            if (s == BigInteger.Zero) return BadRequest("Invalid username.");
            var gs = g * s;

            _logger.LogInformation($"Login attempt for {user}", user, pass);
            return Ok(Convert.ToBase64String(gs.ToByteArray()));
        }

        [HttpGet("{user}/signin/{ticks}/{sign}")]
        public async Task<ActionResult> SignIn([FromRoute] string user, [FromRoute] string ticks, [FromRoute] string sign) {
            var account = await _manager.GetByUser(GetUserId(user));
            if (account == null) return BadRequest("That user does not exist.");
            if (!VerifyChallenge.Check(account.Secret, FromBase64(sign), (long)GetBigInteger(ticks), FromBase64(user), FromBase64(ticks))) {
                _logger.LogInformation($"Unsuccessful login for {user}", user, ticks, sign);
                return BadRequest();
            }

            _logger.LogInformation($"Successful login for {user}", user, ticks, sign);
            return Ok(account.Secret.EncryptStr(account.KeyShare.ToByteArray(true, true)));
        }

        //TODO: there is not verification if the account already exists
        [HttpPost("{user}/signup/{authShare}/{keyShare}/{secret}/{cmkAuth}/{email}")]
        public async Task<TideResponse> SignUp([FromRoute] string user, [FromRoute] string authShare, [FromRoute] string keyShare, [FromRoute] string secret, [FromRoute] string cmkAuth, [FromRoute] string email)
        {
            _logger.LogInformation($"New registration for {user}", user);
            var account = new KeyVault
            {
                User = GetUserId(user),
                AuthShare = GetBigInteger(authShare),
                KeyShare = GetBigInteger(keyShare),
                Secret = AesKey.Parse(FromBase64(secret)),
                CmkAuth = AesKey.Parse(FromBase64(cmkAuth)),
                Email = HttpUtility.UrlDecode(email)
            };

            return await _manager.SetOrUpdate(account);
        }

        [HttpPost("{user}/pass/{authShare}/{secret}/{ticks}/{sign}")]
        public async Task<ActionResult> ChangePass([FromRoute] string user, [FromRoute] string authShare, [FromRoute] string secret, [FromRoute] string ticks, [FromRoute] string sign, [FromQuery] bool withCmk = false)
        {
            var account = await _manager.GetByUser(GetUserId(user));
            var authKey = withCmk ? account.CmkAuth : account.Secret;
            if (!VerifyChallenge.Check(authKey, FromBase64(sign), (long)GetBigInteger(ticks), FromBase64(user), FromBase64(authShare), FromBase64(secret), FromBase64(ticks)))
            {
                _logger.LogInformation($"Unsuccessful change password for {user}", user, authShare, secret, ticks, sign);
                return BadRequest();
            }

            _logger.LogInformation($"Change password for {user}", user);

            account.AuthShare = GetBigInteger(authShare);
            account.Secret = AesKey.Parse(FromBase64(secret));

            await _manager.SetOrUpdate(account);
            return Ok();
        }

        //TODO: Make it last temporarily
        //TODO: Encrypt data with a random key
        [HttpGet("{user}/cmk")]
        public async Task<ActionResult> Recover([FromRoute] string user)
        {
            var account = await _manager.GetByUser(GetUserId(user));
            var share = new OrkShare(_generator.Id, account.KeyShare).ToString();
            var msg = $"You have requested to recover the CMK. Introduce the code [{share}] into tide wallet.";
            
            _mail.SendEmail(user, account.Email, "Key Recovery", msg);
            _logger.LogInformation($"Send cmk share to {user}", user);

            return Ok();
        }

        private byte[] FromBase64(string input) {
            return Convert.FromBase64String(input.DecodeBase64Url());
        }

        private Guid GetUserId(string user) {
            return new Guid(FromBase64(user));
        }

        private BigInteger GetBigInteger(string number) {
            return new BigInteger(FromBase64(number), true, true);
        }
    }
}