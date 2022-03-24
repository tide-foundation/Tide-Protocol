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
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ecc;
using Tide.Encryption.SecretSharing;
using Tide.Encryption.Tools;
using Tide.Ork.Classes;
using Tide.Ork.Components.AuditTrail;
using Tide.Ork.Models;
using Tide.Ork.Repo;
using Tide.VendorSdk.Classes;

namespace Tide.Ork.Controllers
{
    [ApiController]
    [Route("api/cvk")]
    public class CVKController : ControllerBase
    {
        private readonly LoggerPipe _logger;
        private readonly ICvkManager _managerCvk;
        private readonly IRuleManager _ruleManager;
        private readonly IKeyIdManager _keyIdManager;
        private readonly OrkConfig _config;
        private readonly Features _features;

        public CVKController(IKeyManagerFactory factory, ILogger<CVKController> logger, OrkConfig config, Settings settings)
        {
            _managerCvk = factory.BuildManagerCvk();
            _ruleManager = factory.BuildRuleManager();
            _keyIdManager = factory.BuildKeyIdManager();
            _logger = new LoggerPipe(logger, settings, new LoggerConfig());
            _config = config;
            _features = settings.Features;
        }

        [HttpGet("random/{uid}")]
        public ActionResult<CVKRandomResponse> GetRandom([FromQuery] ICollection<Guid> ids)
        {
            if (ids == null || ids.Count < _config.Threshold)
            {
                _logger.LogInformation("Random: The length of the ids ({length}) must be greater than or equal to {threshold}", ids?.Count, _config.Threshold);
                return BadRequest($"The length of the ids must be greater than {_config.Threshold - 1}");
            }

            if (!ids.Contains(_config.Guid)) ids.Add(_config.Guid);

            var idValues = ids.Select(id => new BigInteger(id.ToByteArray(), true, true)).ToList();
            if (idValues.Any(id => id == 0))
            {
                _logger.LogInformation("Random: Ids cannot contain the value zero");
                return BadRequest($"Ids cannot contain the value zero");
            }

            BigInteger cvki;
            using (var rdm = new RandomField(C25519Point.N))
            {
                cvki = rdm.Generate(BigInteger.One);
            }

            var cvks = EccSecretSharing.Share(cvki, idValues, _config.Threshold, C25519Point.N);

            _logger.LogInformation("Random: Generating random for [{orks}]", string.Join(',', ids));
            return new CVKRandomResponse(cvks) {
                CvkPubi = C25519Point.G * cvki
            };
        }

        [HttpPut("random/{uid}")]
        public async Task<ActionResult<byte[]>> AddRandom([FromRoute] Guid vuid, [FromBody] CVKRandRegistrationReq rand)
        {
            if (vuid == Guid.Empty)
            {
                _logger.LogDebug("Random: The vuid must not be empty");
                return BadRequest($"The vuid must not be empty");
            }

            if (rand is null || rand.Shares is null || rand.Shares.Length < _config.Threshold)
            {
                var args = new object[] { rand?.Shares?.Length, _config.Threshold };
                _logger.LogInformation("Random: The length of the shares [length] must be greater than or equal to {threshold}", args);
                return BadRequest($"The length of the ids must be greater than {_config.Threshold - 1}");
            }

            if (rand.Shares.Any(shr => shr.Id != _config.Guid))
            {
                var ids = string.Join(',', rand.Shares.Select(shr => shr.Id).Where(id => id != _config.Guid));
                _logger.LogCritical("Random: Shares were sent to the wrong ORK: {ids}", ids);
                return BadRequest($"Shares were sent to the wrong ORK: '{ids}'");
            }

            var isNew = !await _managerCvk.Exist(vuid);
            if (!isNew)
            {
                _logger.LogInformation("Random: CMK already exists for {uid}", vuid);
                return BadRequest("CMK already exists");
            }

            if (_features.Voucher)
            {
                var signer = await _keyIdManager.GetById(rand.KeyId);
                if (signer == null)
                    return BadRequest("Signer's key must be defined");

                if (!signer.Key.Verify(_config.Guid.ToByteArray().Concat(vuid.ToByteArray()).ToArray(), rand.Signature))
                    return BadRequest("Signature is not valid");
            }

            var account = rand.CreateCvkVault(vuid);
            var resp = await _managerCvk.SetOrUpdate(account);
            if (!resp.Success)
            {
                _logger.LogError($"Random: CVK was not added for uid '{vuid}'");
                return Problem(resp.Error);
            }

            _logger.LogInformation("Random: New CVK for {0} with pub {1}", vuid, rand.CvkPub.ToString());
            var m = Encoding.UTF8.GetBytes(_config.UserName + vuid.ToString());
            return _config.PrivateKey.Sign(m);
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

            if (_features.Voucher) 
            {
                var signer = await _keyIdManager.GetById(keyId);
                if (signer == null)
                    return BadRequest("Signer's key must be defined");

                if (!signer.Key.Verify(_config.Guid.ToByteArray().Concat(vuid.ToByteArray()).ToArray(), signature))
                    return BadRequest("Signature is not valid");
            }

            _logger.LogInformation("New cvk for {0} with pub {1}", vuid, data[0]);

            var resp = await _managerCvk.SetOrUpdate(account);
            if (!resp.Success)
                return resp;
            
            var m = Encoding.UTF8.GetBytes(_config.UserName + vuid.ToString());
            //TODO: The ork should not send the orkid because the client should already know
            var signOrk = Convert.ToBase64String(_config.PrivateKey.Sign(m));
            resp.Content = new { orkid = _config.UserName, sign = signOrk };
            
            return resp;
        }

        [MetricAttribute("cvk", recordSuccess:true)]
        [HttpGet("{vuid}/{token}")]
        public async Task<ActionResult<byte[]>> GetCvk([FromRoute] Guid vuid, [FromRoute] string token, [FromQuery] Guid tranid, [FromQuery] string li = null)
        {
            var lagrangian = BigInteger.Zero;
            if (!string.IsNullOrWhiteSpace(li) && !BigInteger.TryParse(li, out lagrangian)) {
                _logger.LogInformation("Invalid li for {vuid}: '{li}' ", vuid, li);
                return BadRequest("Invalid parameter li");
            }

            var tran = TranToken.Parse(FromBase64(token));

            var account = await _managerCvk.GetById(vuid);
            if (account == null || !tran.Check(account.CvkiAuth, vuid.ToByteArray())) {
                _logger.LoginUnsuccessful(ControllerContext.ActionDescriptor.ControllerName, tranid, vuid, $"Unsuccessful login for {vuid} with {token}");
                return Unauthorized($"Invalid account or signature");
            }

            if (!tran.OnTime) {
                _logger.LoginUnsuccessful(ControllerContext.ActionDescriptor.ControllerName, tranid, vuid, $"Expired token: {token}");
                return StatusCode(408, new TranToken().ToString());
            }

            _logger.LoginSuccessful(ControllerContext.ActionDescriptor.ControllerName, tranid, vuid, $"Returning cvk from {vuid}");
            var cvki = lagrangian <= 0 ? account.CVKi : (account.CVKi * lagrangian).Mod(C25519Point.N);
            return account.CvkiAuth.Encrypt(cvki.ToByteArray(true, true));
        }

        [HttpGet("challenge/{vuid}/{keyId}")]
        public async Task<ActionResult> Challenge([FromRoute] Guid vuid, [FromRoute] Guid keyId)
        {
            var account = await _managerCvk.GetById(vuid);
            if (account == null)
            {
                _logger.LogInformation("Decryption challenge denied for vuid {0} with keyId {1}: account not found.", vuid, keyId);
                return BadRequest($"Denied Challenge for {keyId}");
            }

            var token = TranToken.Generate(account.CvkiAuth);

            var keyPub = await _keyIdManager.GetById(keyId);
            if (keyPub == null) {
                _logger.LogInformation("Decryption challenge denied for vuid {0} with keyId {1}: keyId not found.", vuid, keyId);
                return BadRequest($"Denied Challenge for {keyId}");
            }

            _logger.LogInformation("Decryption challenge granted for vuid {0} with keyId {1}", vuid, keyId);
            var cipher = keyPub.Key.Encrypt(token.GenKey(account.CvkiAuth));
            return Ok(new { Token = token.ToString(), Challenge = cipher.ToString() });
        }

        [HttpPost("plaintext/{vuid}/{keyId}/{token}/{sign}")]
        public async Task<ActionResult> Decrypt([FromRoute] Guid vuid, [FromRoute] Guid keyId, [FromBody] string data, string token, string sign)
        {
            var msgErr = $"Denied data decryption belonging to {vuid}";
            var account = await _managerCvk.GetById(vuid);

            var tran = TranToken.Parse(Convert.FromBase64String(token.DecodeBase64Url()));
            if (!tran.Check(account.CvkiAuth)) {
                _logger.LogInformation("Decryption denied for vuid {0} with keyId {1}: Invalid token.", vuid, keyId);
                return BadRequest(msgErr);
            }

            var keyPub = await _keyIdManager.GetById(keyId);
            if (keyPub == null) {
                _logger.LogInformation("Decryption denied for vuid {0} with keyId {1}: keyId not found.", vuid, keyId);
                return BadRequest(msgErr);
            }

            var buffers = GetBytes(data);
            if (buffers.Any(bff => !Cipher.CheckAsymmetric(bff, account.CvkPub))) {
                _logger.LogInformation("Decryption denied for vuid {0} with keyId {1}: Invalid asymmetric data.", vuid, keyId);
                return BadRequest(msgErr);
            }

            var tags = buffers.Select(bff => Cipher.GetTag(bff)).Distinct().ToList();
            var rules = (await _ruleManager.GetSetBy(account.VuId, tags, keyPub.Id)).Where(rl => rl.Eval()).ToList();
            if (!tags.All(tag => rules.Where(rule => tag == rule.Tag).Any(rule => rule.IsAllowed))) {
                _logger.LogInformation("Decryption denied for vuid {0} with keyId {1}: No rule to allow decryption. Tags: " + string.Join(' ', tags), vuid, keyId);
                return BadRequest(msgErr);
            }

            if (tags.Any(tag => rules.Where(rule => tag == rule.Tag).Any(rule => rule.IsDenied)))
            {
                _logger.LogInformation("Decryption denied for vuid {0} with keyId {1}: Denied by rule", vuid, keyId);
                return BadRequest(msgErr);
            }

            var bufferSign = Convert.FromBase64String(sign.DecodeBase64Url());
            var sessionKey = tran.GenKey(account.CvkiAuth);
            if (!Utils.Equals(sessionKey.Hash(buffers.SelectMany(bff => bff).ToArray()), bufferSign)) {
                _logger.LogInformation("Decryption denied for vuid {0} with keyId {1}: Invalid symmetric data signature.", vuid, keyId);
                return BadRequest(msgErr);
            }

            var c1s =  buffers.Select(bff => Cipher.GetCipherC1(bff)).ToList();
            if (c1s.Any(c1 => !c1.IsValid)) {
                _logger.LogInformation("Decryption denied for vuid {0} with keyId {1}: Invalid data point.", vuid, keyId);
                return BadRequest(msgErr);
            }

            _logger.LogInformation("Decryption granted for vuid {0} with keyId {1}", vuid, keyId);
            var partials = c1s.Select(c1 => (c1 * account.CVKi).ToByteArray())
                .Select(bff => Convert.ToBase64String(sessionKey.Encrypt(bff)));

            return Ok(string.Join(Environment.NewLine, partials));
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

        private BigInteger GetBigInteger(string number)
        {
            return new BigInteger(FromBase64(number), true, true);
        }
        
        static List<byte[]> GetBytes(string data) {
            var line = string.Empty;
            var lst = new List<byte[]>();
            var rdr = new StringReader(data);
            
            while((line = rdr.ReadLine()) != null) {
                lst.Add(Convert.FromBase64String(line.Trim()));
            }

            return lst;
        }

    }
}