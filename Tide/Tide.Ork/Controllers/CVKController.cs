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
    [Route("api/cvk")]
    public class CVKController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ICvkManager _managerCvk;
        private readonly IRuleManager _ruleManager;
        private readonly IKeyIdManager _keyIdManager;
        private readonly OrkConfig _config;


        public CVKController(IKeyManagerFactory factory, ILogger<CVKController> logger, OrkConfig config)
        {
            _managerCvk = factory.BuildManagerCvk();
            _ruleManager = factory.BuildRuleManager();
            _keyIdManager = factory.BuildKeyIdManager();
            _logger = logger;
            _config = config;
        }

        //TODO: there is not verification if the account already exists
        [HttpPut("{vuid}/{keyId}")]
        public async Task<ActionResult<TideResponse>> Add([FromRoute] Guid vuid, [FromRoute] Guid keyId, [FromBody] string[] data)
        {
            var signature = FromBase64(data[3]);
            var account = new CvkVault
            {
                VuId = vuid,
                CvkPub = C25519Key.Parse(FromBase64(data[0])),
                CVKi = GetBigInteger(data[1]),
                CvkiAuth = AesKey.Parse(FromBase64(data[2]))
            };

            var signer = await _keyIdManager.GetById(keyId);
            if (signer == null)
                return BadRequest("Signer's key must be defined");

            if (!signer.Key.Verify(_config.Guid.ToByteArray().Concat(vuid.ToByteArray()).ToArray(), signature))
                return BadRequest("Signature is not valid ");

            _logger.LogInformation($"New cvk for {vuid} with share {data[1]}", vuid, data[0]);

            return await _managerCvk.SetOrUpdate(account);
        }

        [HttpGet("{vuid}/{token}")]
        public async Task<ActionResult<byte[]>> GetCvk([FromRoute] Guid vuid, [FromRoute] string token)
        {
            var tran = TranToken.Parse(FromBase64(token));

            var account = await _managerCvk.GetById(vuid);
            if (account == null || !tran.Check(account.CvkiAuth, vuid.ToByteArray()))
                return _logger.Log(Unauthorized($"Invalid account or signature"),
                    $"Unsuccessful login for {vuid} with {token}");

            if (!tran.OnTime)
                return _logger.Log(StatusCode(408, new TranToken().ToString()), $"Expired token: {token}");

            _logger.LogInformation($"Returning cvk from {vuid}", vuid, token);
            return account.CvkiAuth.Encrypt(account.CVKi.ToByteArray(true, true));
        }

        [HttpGet("challenge/{vuid}/{keyId}")]
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

        [HttpGet("plaintext/{vuid}/{keyId}/{data}/{token}/{sign}")]
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

        [HttpPost("{vuid}")]
        public async Task<ActionResult> Confirm([FromRoute] Guid vuid)
        {
            await _managerCvk.Confirm(vuid);
            await _ruleManager.ConfirmAll(vuid);
            _logger.LogInformation($"Confimed vuid {vuid}", vuid);
            return Ok();
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