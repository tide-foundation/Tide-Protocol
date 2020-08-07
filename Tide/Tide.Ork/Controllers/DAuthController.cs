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
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ecc;
using Tide.Encryption.Tools;
using Tide.Ork.Classes;
using Tide.VendorSdk.Classes;

namespace Tide.Ork.Controllers
{
    [ApiController]
    [Route("api/dauth")]
    public class DAuthController : ControllerBase
    {
        private readonly IEmailClient _mail;
        private readonly ILogger _logger;
        private readonly IKeyManager _manager;
        private readonly IManager<CvkVault> _managerCvk;
        private readonly IRuleManager _ruleManager;
        private readonly IManager<KeyIdVault> _keyIdManager;

        public DAuthController(IKeyManagerFactory factory, IEmailClient mail, ILogger<DAuthController> logger)
        {
            _manager = factory.BuildManager();
            _managerCvk = factory.BuildManagerCvk();
            _ruleManager = factory.BuildRuleManager();
            _keyIdManager = factory.BuildKeyIdManager();
            _mail = mail;
            _logger = logger;
        }

        [HttpGet("{uid}/convert/{pass}")]
        public async Task<ActionResult<string>> ConvertPass([FromRoute] Guid uid, [FromRoute] string pass)
        {
            var g = C25519Point.From(Convert.FromBase64String(pass.DecodeBase64Url()));
            if (!g.IsValid) return BadRequest();

            var s = await _manager.GetPrism(uid);
            if (s == BigInteger.Zero) return BadRequest("Invalid username.");
            var gs = g * s;

            _logger.LogInformation($"Login attempt for {uid}", uid, pass);
            return Ok(Convert.ToBase64String(gs.ToByteArray()));
        }

        [HttpGet("{uid}/authenticate/{ticks}/{sign}")]
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

        //TODO: Move secrets out of the url
        //TODO: there is not verification if the account already exists
        [HttpPost("{uid}/signup/{prism}/{cmk}/{prismAuth}/{cmkAuth}/{email}")]
        public async Task<TideResponse> SignUp([FromRoute] Guid uid, [FromRoute] string prism, [FromRoute] string cmk, [FromRoute] string prismAuth, [FromRoute] string cmkAuth, [FromRoute] string email)
        {
            _logger.LogInformation($"New registration for {uid}", uid);
            var account = new KeyVault
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

        [HttpPost("{uid}/pass/{prism}/{prismAuth}/{ticks}/{sign}")]
        public async Task<ActionResult> ChangePass([FromRoute] Guid uid, [FromRoute] string prism, [FromRoute] string prismAuth, [FromRoute] long ticks, [FromRoute] string sign, [FromQuery] bool withCmk = false)
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
        [HttpGet("{uid}/cmk")]
        public async Task<ActionResult> Recover([FromRoute] Guid uid)
        {
            var account = await _manager.GetById(uid);
            var generator = IdGenerator.Seed(new Uri(Request.GetDisplayUrl()));
            var share = new OrkShare(generator.Id, account.Cmki).ToString();
            var msg = $"You have requested to recover the CMK. Introduce the code [{share}] into tide wallet.";

            _mail.SendEmail(uid.ToString(), account.Email, "Key Recovery", msg);
            _logger.LogInformation($"Send cmk share to {uid}", uid);

            return Ok();
        }

        //TODO: there is not verification if the account already exists
        [HttpPost("{vuid}/cvk")]
        public async Task<TideResponse> RegisterCvk([FromRoute] Guid vuid, [FromBody] string[] data)
        {
            var account = new CvkVault
            {
                VuId = vuid,
                CvkPub = C25519Key.Parse(FromBase64(data[0])),
                CVKi = GetBigInteger(data[1]),
                CvkiAuth = AesKey.Parse(FromBase64(data[2])),
            };

            _logger.LogInformation($"New cvk for {vuid} with share {data[1]}", vuid, data[0]);

            return await _managerCvk.SetOrUpdate(account);
        }

        [HttpGet("{vuid}/challenge")]
        public async Task<ActionResult> ChallengeVendor([FromRoute] Guid vuid)
        {
            var account = await _managerCvk.GetById(vuid);
            var token = TranToken.Generate(account.CvkiAuth);

            var cipher = account.CvkPub.Encrypt(token.GenKey(account.CvkiAuth));
            _logger.LogInformation($"Challenge from {vuid}", vuid, cipher.ToString());

            return Ok(new { Token = token.ToString(), Challenge = cipher.ToString() });
        }


        [HttpGet("{vuid}/challenge/{keyId}")]
        public async Task<ActionResult> Challenge([FromRoute] Guid vuid, [FromRoute] Guid keyId)
        {
            _logger.LogInformation($"Challenge from {vuid}", vuid, keyId);

            var account = await _managerCvk.GetById(vuid);
            var token = TranToken.Generate(account.CvkiAuth);

            var keyPub = await _keyIdManager.GetById(keyId);
            if (keyPub == null)
                return Deny($"Denied Challenge for {keyId}");

            var cipher = keyPub.Key.Encrypt(token.GenKey(account.CvkiAuth));

            return Ok(new { Token = token.ToString(), Challenge = cipher.ToString() });
        }

        [HttpGet("{vuid}/decrypt/{keyId}/{data}/{token}/{sign}")]
        public async Task<ActionResult> Decrypt([FromRoute] Guid vuid, [FromRoute] Guid keyId, string data, string token, string sign)
        {
            var msgErr = $"Denied data decryption belonging to {vuid}";
            var account = await _managerCvk.GetById(vuid);

            var tran = TranToken.Parse(Convert.FromBase64String(token.DecodeBase64Url()));
            if (!tran.Check(account.CvkiAuth)) return Deny(msgErr);

            var keyPub = await _keyIdManager.GetById(keyId);
            if (keyPub == null)
                return Deny(msgErr);

            var dataBuffer = Convert.FromBase64String(data.DecodeBase64Url());
            if (!Cipher.CheckAsymmetric(dataBuffer, account.CvkPub))
                return Deny(msgErr);

            var tag = Cipher.GetTag(dataBuffer);
            var rules = await _ruleManager.GetSetBy(account.VuId, tag, keyPub.Id);
            if (!rules.Any(rule => rule.Apply() && rule.Action == RuleAction.Allow))
                return Deny(msgErr);

            if (rules.Any(rule => rule.Apply() && rule.Action == RuleAction.Deny))
                return Deny(msgErr);

            var bufferSign = Convert.FromBase64String(sign.DecodeBase64Url());
            var sessionKey = tran.GenKey(account.CvkiAuth);
            if (!Utils.Equals(sessionKey.Hash(dataBuffer), bufferSign)) return Deny(msgErr);

            var c1 = Cipher.GetCipherC1(dataBuffer);
            if (!c1.IsValid) return Deny(msgErr);

            var cipher = sessionKey.Encrypt((c1 * account.CVKi).ToByteArray());

            _logger.LogInformation($"Decrypt data belonging to {vuid}", vuid, data, token);
            return Ok(Convert.ToBase64String(cipher));
        }

        private byte[] FromBase64(string input)
        {
            return Convert.FromBase64String(input.DecodeBase64Url());
        }

        private ActionResult Deny(string message, params object[] args)
        {
            _logger.LogInformation(message, args);
            return BadRequest();
        }

        private BigInteger GetBigInteger(string number)
        {
            return new BigInteger(FromBase64(number), true, true);
        }
    }
}