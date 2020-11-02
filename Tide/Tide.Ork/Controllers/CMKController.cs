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
using Tide.Ork.Models;
using Tide.Ork.Repo;

namespace Tide.Ork.Controllers
{
    [ApiController]
    [Route("api/cmk")]
    public class CMKController : ControllerBase
    {
        private readonly IEmailClient _mail;
        private readonly ILogger _logger;
        private readonly ICmkManager _manager;
        private readonly OrkConfig _config;

        public CMKController(IKeyManagerFactory factory, IEmailClient mail, ILogger<CMKController> logger, OrkConfig config)
        {
            _manager = factory.BuildCmkManager();
            _mail = mail;
            _logger = logger;
            _config = config;
        }

        //TODO: Move secrets out of the url
        //TODO: there is not verification if the account already exists
        [HttpPut("{uid}/{prism}/{cmk}/{prismAuth}/{cmkAuth}/{email}")]
        public async Task<TideResponse> Add([FromRoute] Guid uid, [FromRoute] string prism, [FromRoute] string cmk, [FromRoute] string prismAuth, [FromRoute] string cmkAuth, [FromRoute] string email)
        {
            _logger.LogInformation($"New registration for {uid}", uid);
            var account = new CmkVault
            {
                UserId = uid,
                Prismi = GetBigInteger(prism),
                Cmki = GetBigInteger(cmk),
                PrismiAuth = AesKey.Parse(FromBase64(prismAuth)),
                CmkiAuth = AesKey.Parse(FromBase64(cmkAuth)),
                Email = HttpUtility.UrlDecode(email)
            };

            return await _manager.SetOrUpdate(account);
        }

        [ThrottleAttribute("uid")]
        [HttpGet("prism/{uid}/{pass}")]
        public async Task<ActionResult<ApplyResponse>> Apply([FromRoute] Guid uid, [FromRoute] string pass)
        {
            if (!pass.FromBase64UrlString(out byte[] bytesPass)) {
                _logger.LogInformation($"Apply: Invalid pass for {uid}");
                return BadRequest("Invalid parameters");
            }

            C25519Point g;
            try
            {
                g = C25519Point.From(bytesPass);
                if (!g.IsValid) {
                    _logger.LogInformation($"Apply: Invalid point for {uid}");
                    return BadRequest("Invalid parameters");
                }
            }
            catch (ArgumentException)
            {
                _logger.LogInformation($"Apply: Invalid point for {uid} with error");
                return BadRequest("Invalid parameters");
            }

            var s = await _manager.GetPrism(uid);
            if (s == BigInteger.Zero) {
                _logger.LogInformation($"Apply: Account {uid} does not exist");
                return BadRequest("Invalid parameters");
            }

            var gs = g * s;

            _logger.LogInformation($"Login attempt for {uid}", uid, pass);
            return new ApplyResponse
            {
                Prism = gs.ToByteArray(),
                Token = new TranToken().ToByteArray()
            };
        }

        //TODO: Add throttling by ip and account separate
        [HttpGet("auth/{uid}/{token}")]
        public async Task<ActionResult> Authenticate([FromRoute] Guid uid, [FromRoute] string token)
        {
            if (!token.FromBase64UrlString(out byte[] bytesToken))
            {
                _logger.LogInformation($"Authenticate: Invalid token format for {uid}");
                return Unauthorized();
            }

            var tran = TranToken.Parse(bytesToken);
            var account = await _manager.GetById(uid);
            if (account == null || tran == null || !tran.Check(account.PrismiAuth, uid.ToByteArray())) {
                if (account == null)
                    _logger.LogInformation($"Authenticate: Account {uid} does not exist");
                else
                    _logger.LogInformation($"Authenticate: Invalid token for {uid}");

                return Unauthorized("Invalid account or signature");
            }
            
            if (!tran.OnTime) {
                _logger.LogInformation($"Authenticate: Expired token for {uid}");
                return StatusCode(418, new TranToken().ToString());
            }

            _logger.LogInformation($"Authenticate: Successful login for {uid}");
            return Ok(account.PrismiAuth.EncryptStr(account.Cmki.ToByteArray(true, true)));
        }

        [HttpPost("prism/{uid}/{prism}/{prismAuth}/{token}")]
        public async Task<ActionResult> ChangePrism([FromRoute] Guid uid, [FromRoute] string prism, [FromRoute] string prismAuth, [FromRoute] string token, [FromQuery] bool withCmk = false)
        {
            var tran = TranToken.Parse(FromBase64(token));
            var toCheck = uid.ToByteArray().Concat(FromBase64(prism)).Concat(FromBase64(prismAuth)).ToArray();

            var account = await _manager.GetById(uid);
            if (account == null)
                return _logger.Log(Unauthorized($"Unsuccessful change password for {uid}"),
                    $"Unsuccessful change password for {uid}. Account was not found");

            var authKey = withCmk ? account.CmkiAuth : account.PrismiAuth;
            if (!tran.Check(authKey, toCheck))
                return _logger.Log(Unauthorized($"Unsuccessful change password for {uid}"),
                    $"Unsuccessful change password for {uid} with {token}");

            _logger.LogInformation($"Change password for {uid}", uid);

            account.Prismi = GetBigInteger(prism);
            account.PrismiAuth = AesKey.Parse(FromBase64(prismAuth));

            await _manager.SetOrUpdate(account);
            return Ok();
        }

        //TODO: Make it last temporarily
        //TODO: Encrypt data with a random key
        [HttpGet("mail/{uid}")]
        public async Task<ActionResult> Recover([FromRoute] Guid uid)
        {
            var account = await _manager.GetById(uid);
            var share = new OrkShare(_config.Id, account.Cmki).ToString();
            var msg = $"You have requested to recover the CMK. Introduce the code [{share}] into tide wallet.";

            _mail.SendEmail(uid.ToString(), account.Email, "Key Recovery", msg);
            _logger.LogInformation($"Send cmk share to {uid}", uid);

            return Ok();
        }

        [HttpPost("{uid}")]
        public async Task<ActionResult> Confirm([FromRoute] Guid uid)
        {
            await _manager.Confirm(uid);
            _logger.LogInformation($"Confimed user {uid}", uid);
            return Ok();
        }

        private byte[] FromBase64(string input)
        {
            return Convert.FromBase64String(input.DecodeBase64Url());
        }

        private BigInteger GetBigInteger(string number)
        {
            return new BigInteger(FromBase64(number), true, true);
        }
    }
}