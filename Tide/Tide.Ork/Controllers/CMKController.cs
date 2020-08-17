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
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ecc;
using Tide.Encryption.Tools;
using Tide.Ork.Classes;
using Tide.Ork.Repo;
using Tide.VendorSdk.Classes;

namespace Tide.Ork.Controllers
{
    [ApiController]
    [Route("api/cmk")]
    public class CMKController : ControllerBase
    {
        private readonly IEmailClient _mail;
        private readonly ILogger _logger;
        private readonly ICmkManager _manager;

        private IdGenerator IdGen => IdGenerator.Seed(new Uri(Request.GetDisplayUrl()));

        public CMKController(IKeyManagerFactory factory, IEmailClient mail, ILogger<CMKController> logger)
        {
            _manager = factory.BuildCmkManager();
            _mail = mail;
            _logger = logger;
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

        [HttpGet("prism/{uid}/{pass}")]
        public async Task<ActionResult<string>> Apply([FromRoute] Guid uid, [FromRoute] string pass)
        {
            var g = C25519Point.From(Convert.FromBase64String(pass.DecodeBase64Url()));
            if (!g.IsValid) return BadRequest();

            var s = await _manager.GetPrism(uid);
            if (s == BigInteger.Zero) return BadRequest("Invalid username.");
            var gs = g * s;

            _logger.LogInformation($"Login attempt for {uid}", uid, pass);
            return Ok(Convert.ToBase64String(gs.ToByteArray()));
        }

        [HttpGet("auth/{uid}/{ticks}/{sign}")]
        public async Task<ActionResult> Authenticate([FromRoute] Guid uid, [FromRoute] long ticks, [FromRoute] string sign)
        {
            var account = await _manager.GetById(uid);
            if (account == null) return BadRequest("That user does not exist.");
            if (!VerifyChallenge.Check(account.PrismiAuth, FromBase64(sign), ticks, uid.ToByteArray(), BitConverter.GetBytes(ticks)))
            {
                _logger.LogInformation($"Unsuccessful login for {uid}", uid, ticks, sign);
                return BadRequest();
            }

            _logger.LogInformation($"Successful login for {uid}", uid, ticks, sign);
            return Ok(account.PrismiAuth.EncryptStr(account.Cmki.ToByteArray(true, true)));
        }

        [HttpPost("prism/{uid}/{prism}/{prismAuth}/{ticks}/{sign}")]
        public async Task<ActionResult> ChangePrism([FromRoute] Guid uid, [FromRoute] string prism, [FromRoute] string prismAuth, [FromRoute] long ticks, [FromRoute] string sign, [FromQuery] bool withCmk = false)
        {
            var account = await _manager.GetById(uid);
            var authKey = withCmk ? account.CmkiAuth : account.PrismiAuth;
            if (!VerifyChallenge.Check(authKey, FromBase64(sign), ticks, uid.ToByteArray(), FromBase64(prism), FromBase64(prismAuth), BitConverter.GetBytes(ticks)))
            {
                _logger.LogInformation($"Unsuccessful change password for {uid}", uid, prism, prismAuth, ticks, sign);
                return BadRequest();
            }

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
            var share = new OrkShare(IdGen.Id, account.Cmki).ToString();
            var msg = $"You have requested to recover the CMK. Introduce the code [{share}] into tide wallet.";

            _mail.SendEmail(uid.ToString(), account.Email, "Key Recovery", msg);
            _logger.LogInformation($"Send cmk share to {uid}", uid);

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