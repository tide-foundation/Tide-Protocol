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
using Tide.Encryption.Ed;
using Tide.Encryption.Tools;
using Tide.Encryption.SecretSharing;
using Tide.Ork.Classes;
using Tide.Ork.Components.AuditTrail;
using Tide.Ork.Models;
using Tide.Ork.Repo;
using Tide.VendorSdk.Classes;
using System.Text.Json;

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
        private readonly SimulatorOrkManager _orkManager; 
        private readonly string _orkId;
        private readonly KeyGenerator _keyGenerator;

        public CVKController(IKeyManagerFactory factory, ILogger<CVKController> logger, OrkConfig config, Settings settings)
        {
            _managerCvk = factory.BuildManagerCvk();
            _ruleManager = factory.BuildRuleManager();
            _keyIdManager = factory.BuildKeyIdManager();
            _logger = new LoggerPipe(logger, settings, new LoggerConfig());
            _config = config;
            _features = settings.Features;
            _orkId = settings.Instance.Username;
             var cln = new Tide.Ork.Classes.SimulatorClient(settings.Endpoints.Simulator.Api, _orkId, settings.Instance.GetPrivateKey());
            _orkManager = new SimulatorOrkManager(_orkId, cln);
            _keyGenerator = new KeyGenerator(_config.PrivateKey.X, _config.PrivateKey.GetPublic().Y, _config.UserName, _config.Threshold);
        }

        [HttpGet("random/{vuid}")]
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

            BigInteger cvki,cvk2i;
            using (var rdm = new RandomField(Ed25519.N))
            {
                cvki = rdm.Generate(BigInteger.One);
                cvk2i =rdm.Generate(BigInteger.One);
            }
            var cvkPubi = Ed25519.G * cvki;
            var cvk2Pubi = Ed25519.G * cvk2i;
            var cvks = EccSecretSharing.Share(cvki, idValues, _config.Threshold, Ed25519.N);
            var cvk2s = EccSecretSharing.Share(cvk2i, idValues, _config.Threshold, Ed25519.N);

            _logger.LogInformation("Random: Generating random for [{orks}]", string.Join(',', ids));
            return new CVKRandomResponse(_config.UserName,cvkPubi,cvk2Pubi, cvks,cvk2s,cvki,cvk2i) {
                
            };
        }

        
        [HttpGet("genshard/{vuid}")]
        public async Task<ActionResult> GenShard([FromRoute] Guid vuid, [FromQuery] string numKeys, [FromQuery] Ed25519Point signi, [FromQuery] Ed25519Point gVoucher, [FromQuery] ICollection<string> orkIds)
        {
            // Get ork Publics from simulator, searching with their usernames e.g. ork1
            var orkPubTasks = orkIds.Select(mIdORKj => GetPubByOrkId(mIdORKj)); 
            
            Ed25519Key[] mgOrkj_Keys = await Task.WhenAll(orkPubTasks); // wait for tasks to end
            
            return Ok(_keyGenerator.GenShard(vuid.ToString(), mgOrkj_Keys, Int32.Parse(numKeys), null, orkIds.ToArray()));
        }


        [HttpGet("set/{vuid}")]
        public async Task<ActionResult> SetCVK([FromRoute] Guid vuid, [FromQuery] string CVKtimestamp , [FromQuery] ICollection<string> orkIds, [FromBody] ICollection<string> yijCipher)
        {
           // Get ork Publics from simulator, searching with their usernames e.g. ork1
            var orkPubTasks = orkIds.Select(mIdORKj => GetPubByOrkId(mIdORKj));
            Ed25519Key[] mgOrkj_Keys = await Task.WhenAll(orkPubTasks); // wait for tasks to end

            string response;
            try{
                response = _keyGenerator.SetKey(vuid.ToString(), yijCipher.ToArray(), mgOrkj_Keys);
            }catch(Exception e){
                _logger.LogInformation($"SetCVK: {e}", e);
                return BadRequest(e);
            }

            return Ok(response);
        }


        [HttpGet("precommit/{vuid}")]
        public async Task<ActionResult> PreCommit([FromRoute] Guid uid, [FromQuery] Ed25519Point R2, [FromQuery] Ed25519Point gCVKtest, [FromQuery] Ed25519Point gCVK2test, [FromQuery] ICollection<string> orkIds, [FromBody] string encryptedState)
        {
            // Get ork Publics from simulator, searching with their usernames e.g. ork1
            var orkPubTasks = orkIds.Select(mIdORKj => GetPubByOrkId(mIdORKj));
            Ed25519Key[] mgOrkj_Keys = await Task.WhenAll(orkPubTasks); // wait for tasks to end

            KeyGenerator.PreCommitResponse preCommitResponse;
            var gKtest = new Ed25519Point[]{gCVKtest, gCVK2test};

            try{
                preCommitResponse = _keyGenerator.PreCommit(uid.ToString(), gKtest, mgOrkj_Keys, R2, encryptedState);
            }catch(Exception e){
                _logger.LogInformation($"Commit: {e}", e);
                return BadRequest(e);
            }

            return Ok(preCommitResponse.ToString());
        }

        // [HttpPut("[commit]/{vuid}")]
        // public async Task<ActionResult<string>> Commit([FromRoute] Guid vuid, [FromBody] string data)
        // {
            

            
            
            
            
        //     var account = new CvkVault
        //     {
        //         VuId = vuid,
        //         GCVK =    ,
        //         CVKi = , 
        //         GCvkAuth = ,
        //         CVK2i = ,
        //         GCVK2 =  
                          
        //     };
        //     var resp = await _managerCvk.SetOrUpdate(account);
        //     if (!resp.Success) {
        //         _logger.LogInformation($"Commit: CMK was not added for uid '{vuid}'");
        //         return Problem(resp.Error);
        //     }

        //     return Ok();
        // }


        // [HttpPut("random/{vuid}/{partialCvkPub}/{partialCvk2Pub}")]
        // public async Task<ActionResult<CvkRandomResponseAdd>> AddRandom([FromRoute] Guid vuid, [FromRoute] Ed25519Point partialCvkPub, [FromRoute] Ed25519Point partialCvk2Pub, [FromBody] CVKRandRegistrationReq rand,[FromQuery] string li = null)
        // {
        //     if (vuid == Guid.Empty)
        //     {
        //         _logger.LogDebug("Random: The vuid must not be empty");
        //         return BadRequest($"The vuid must not be empty");
        //     }

        //     if (rand is null || rand.Shares is null || rand.Shares.Length < _config.Threshold)
        //     {
        //         var args = new object[] { rand?.Shares?.Length, _config.Threshold };
        //         _logger.LogInformation("Random: The length of the shares [length] must be greater than or equal to {threshold}", args);
        //         return BadRequest($"The length of the ids must be greater than {_config.Threshold - 1}");
        //     }

        //     if (rand.Shares.Any(shr => shr.Id != _config.Guid))
        //     {
        //         var ids = string.Join(',', rand.Shares.Select(shr => shr.Id).Where(id => id != _config.Guid));
        //         _logger.LogCritical("Random: Shares were sent to the wrong ORK: {ids}", ids);
        //         return BadRequest($"Shares were sent to the wrong ORK: '{ids}'");
        //     }

        //     var lagrangian = BigInteger.Zero;
        //     if (!string.IsNullOrWhiteSpace(li) && !BigInteger.TryParse(li, out lagrangian))
        //     {
        //         _logger.LogInformation("AddRandom: Invalid li for {uid}: '{li}' ", vuid, li);
        //         return BadRequest("Invalid parameter li");
        //     }
        //     var isNew = !await _managerCvk.Exist(vuid);
        //     if (!isNew)
        //     {
        //         _logger.LogInformation("Random: CMK already exists for {uid}", vuid);
        //         return BadRequest("CMK already exists");
        //     }

        //     if (_features.Voucher)
        //     {
        //         var signer = await _keyIdManager.GetById(rand.KeyId);
        //         if (signer == null)
        //             return BadRequest("Signer's key must be defined");

        //         if (!signer.Key.EdDSAVerify(_config.Guid.ToByteArray().Concat(vuid.ToByteArray()).ToArray(), rand.Signature))
        //             return BadRequest("Signature is not valid");
        //     }

        //     var account = new CvkVault
        //     {
        //         VuId = vuid,
        //         CVKi = rand.ComputeCvk(),
        //         CVK2i = rand.ComputeCvk2(),
        //         CvkPub = new Ed25519Key(Ed25519.G *(rand.ComputeCvk() * lagrangian)),
        //         CvkiAuth = rand.CvkiAuth
        //     };
        //     var resp = await _managerCvk.SetOrUpdate(account);
        //     if (!resp.Success)
        //     {
        //         _logger.LogError($"Random: CVK was not added for uid '{vuid}'");
        //         return Problem(resp.Error);
        //     }

        //     _logger.LogInformation("Random: New CVK for {0} with pub {1}", vuid, account.CvkPub);
        
        //     var cvkPub = partialCvkPub + (Ed25519.G * rand.GetCvki());
        //     var cvk2Pub = partialCvk2Pub + (Ed25519.G * rand.GetCvk2i());

        //     var s = Ed25519Dsa.Sign(rand.GetCvk2i() , rand.GetCvki() , cvkPub, cvk2Pub, rand.GetEntry().MessageSignedBytes());


        //     var m = Encoding.UTF8.GetBytes(_config.UserName + vuid.ToString());
        //     var token = new TranToken();
        //     token.Sign(_config.SecretKey); // token client will use to authetnicate on SignEntry endpoint
        //     return new CvkRandomResponseAdd
        //     {
        //         CvkPub = Ed25519.G * (rand.ComputeCvk() * lagrangian ),
        //         Cvk2Pub = Ed25519.G * (rand.ComputeCvk2() * lagrangian),
        //         Signature = new { orkid = _config.UserName, sign = Convert.ToBase64String(_config.PrivateKey.EdDSASign(m))}, // OrkSign type
        //         EncryptedToken = account.CvkiAuth.Encrypt(token.ToByteArray()),
        //         S = s.ToString()
        //     };
        // }

        // //TODO: there is not verification if the account already exists
        // [HttpPut("{vuid}/{keyId}")]
        // public async Task<ActionResult<TideResponse>> Add([FromRoute] Guid vuid, [FromRoute] Guid keyId, [FromBody] string[] data)
        // {
        //     var signature = FromBase64(data[3]);
        //     var account = new CvkVault
        //     {
        //         VuId = vuid,
        //         CvkPub = Ed25519Key.ParsePublic(FromBase64(data[0])),
        //         CVKi = GetBigInteger(data[1]),
        //         CvkiAuth = AesKey.Parse(FromBase64(data[2]))
        //     };

        //     if (_features.Voucher) 
        //     {
        //         var signer = await _keyIdManager.GetById(keyId);
        //         if (signer == null)
        //             return BadRequest("Signer's key must be defined");

        //         if (!signer.Key.EdDSAVerify(_config.Guid.ToByteArray().Concat(vuid.ToByteArray()).ToArray(), signature))
        //             return BadRequest("Signature is not valid");
        //     }

        //     _logger.LogInformation("New cvk for {0} with pub {1}", vuid, data[0]);

        //     var resp = await _managerCvk.SetOrUpdate(account);
        //     if (!resp.Success)
        //         return resp;
            
        //     var m = Encoding.UTF8.GetBytes(_config.UserName + vuid.ToString());
        //     //TODO: The ork should not send the orkid because the client should already know
        //     var signOrk = Convert.ToBase64String(_config.PrivateKey.EdDSASign(m));
        //     resp.Content = new { orkid = _config.UserName, sign = signOrk };
            
        //     return resp;
        // }

        // [MetricAttribute("cvk", recordSuccess:true)]
        // [HttpGet("signin/{vuid}/{gRmul}/{s}/{timestamp2}/{gSessKeyPub}/{challenge}/{gCMKAuth}/{gCVKR}/{gCVK}")]
        // public async Task<ActionResult<byte[]>> SignCvk([FromRoute] Guid vuid, [FromRoute] Ed25519Point gRmul, [FromRoute] string s, [FromRoute] long timestamp2 , [FromRoute] Ed25519Point gSessKeyPub, [FromRoute] string challenge,[FromRoute] Ed25519Point gCMKAuth,[FromRoute] Ed25519Point gCVKR,[FromRoute] Ed25519Point gCVK)//remove cmkAuth  and gCVK later
        // {
        //     var account = await _managerCvk.GetById(vuid);
        //     if (account == null) {
        //         _logger.LoginUnsuccessful(ControllerContext.ActionDescriptor.ControllerName, vuid, vuid, $"Unsuccessful login for {vuid}");
        //         return Unauthorized($"Invalid account");
        //     }

        //     //Verify timestamp2 in recent (10 min)
        //     var Time= DateTime.FromBinary(timestamp2);
        //     const long _window = TimeSpan.TicksPerHour; //Check later

        //     if(!(Time >= DateTime.UtcNow.AddTicks(-_window) && Time <= DateTime.UtcNow.AddTicks(_window))){
        //         _logger.LoginUnsuccessful(ControllerContext.ActionDescriptor.ControllerName, vuid, vuid, $"Expired");
        //         return StatusCode(408, new TranToken().ToString()); //Return this???
        //     }

        //     if (!gRmul.IsSafePoint() || !gSessKeyPub.IsSafePoint()) {
        //         _logger.LogInformation($"Apply: Invalid point for {vuid}");
        //         return BadRequest("Invalid parameters");
        //     }
            
        //     var S = BigInteger.Parse(s);
        //     if(S == BigInteger.Zero || S == Ed25519.N) {
        //         _logger.LogInformation($"Apply: Invalid s for {vuid}");
        //         return BadRequest("Invalid parameters");
        //     }

        //     var CMK_ToHashM = Encoding.ASCII.GetBytes(timestamp2.ToString() + Convert.ToBase64String(gSessKeyPub.ToByteArray())).ToArray();
        //     var CMK_M = Utils.Hash(CMK_ToHashM);

        //     var CmkAuthHash = new BigInteger(Utils.Hash(Encoding.ASCII.GetBytes("CMK authentication")), true, false).Mod(Ed25519.N);

        //     var ToHashH = gCMKAuth.ToByteArray().Concat(CMK_M).ToArray(); // add account.gCMKAuth 
        //     var H = new BigInteger(Utils.Hash(ToHashH), true, false).Mod(Ed25519.N);

        //     var _8N = BigInteger.Parse("8");
        //     if(!(Ed25519.G * S * _8N).GetX().Equals((gRmul * _8N  +  (gCMKAuth * H * CmkAuthHash * _8N)).GetX())){ 
        //         _logger.LogInformation($"Apply: Invalid Blind Signature for {vuid}");
        //         return BadRequest("Some consistent garbage");
        //     }

        //     /// Standard EdDSA signature to sign challenge with CVKi from here on

        //     var CVK_M = Encoding.ASCII.GetBytes(challenge).ToArray(); // perform JWT checking here first
            
        //     /// From RFC 8032 5.1.6.2:
        //     /// Compute SHA-512(dom2(F, C) || prefix || PH(M)), where M is the
        //     /// message to be signed.  Interpret the 64-octet digest as a little-
        //     /// endian integer r.
        //     ///
        //     /// prefix : CVKRi   r : CVKRi
        //     var CVKRi_ToHash = account.CVK2i.ToByteArray(true, false).Concat(CVK_M).ToArray();    
        //     var CVKRi = new BigInteger(Utils.HashSHA512(CVKRi_ToHash), true, false).Mod(Ed25519.N);
            
        //     /// From RFC 8032 5.1.6.4:
        //     /// Compute SHA512(dom2(F, C) || R || A || PH(M)), and interpret the
        //     /// 64-octet digest as a little-endian integer k.
        //     ///
        //     /// R : gCVKR     A : gCVK     PH(M) : challenge    k : CVKH
        //     var CVKH_ToHash = Ed25519Dsa.EncodeEd25519Point(gCVKR).Concat(Ed25519Dsa.EncodeEd25519Point(gCVK)).Concat(CVK_M).ToArray(); // implement point compression
        //     var CVKH = new BigInteger(Utils.HashSHA512(CVKH_ToHash), true, false).Mod(Ed25519.N);
            
        //     /// From RFC 8032 5.1.6.5:
        //     /// Compute S = (r + k * s) mod L.  For efficiency, again reduce k
        //     /// modulo L first.
        //     ///
        //     /// r: CVKRi    k : CVKH    s : CVKi
        //     var CVKSi = (CVKRi + (CVKH * account.CVKi)).Mod(Ed25519.N);
            
        //     var ECDH_seed = Utils.Hash((gSessKeyPub * _config.PrivateKey.X).ToByteArray()); // CHECK THIS WORKS
        //     var ECDHi = AesKey.Seed(ECDH_seed);

        //     _logger.LogInformation($"SignCvk: Returned challenge signature for VUID: {vuid}");
        //     // No need to return R : gCVKR as we already have it
        //     return Ok(ECDHi.EncryptStr(CVKSi.ToByteArray(true, false)));
        // }

        // [HttpGet("pre/{vuid}/{timestamp2}/{gSessKeyPub}/{challenge}")]
        // public async Task<ActionResult<byte[]>> PreSignCvk([FromRoute] Guid vuid, [FromRoute] string timestamp2 , [FromRoute] Ed25519Point gSessKeyPub, [FromRoute] string challenge)
        // {
        //     var account = await _managerCvk.GetById(vuid); // find hardcoded vuid
        //     if (account == null) {
        //         _logger.LoginUnsuccessful(ControllerContext.ActionDescriptor.ControllerName, null, vuid, $"invalid VUID: {vuid}");
        //         return Unauthorized($"Invalid account");
        //     }
            
        //     var M_bytes = Encoding.ASCII.GetBytes(challenge).ToArray();
            
        //     var CVKRi_ToHash = account.CVK2i.ToByteArray(true, false).Concat(M_bytes).ToArray();
        //     var CVKRi = new BigInteger(Utils.HashSHA512(CVKRi_ToHash), true, false).Mod(Ed25519.N);
        //     var gCVKRi = Ed25519.G * CVKRi;
            
        //     var ECDH_seed = Utils.Hash((gSessKeyPub * _config.PrivateKey.X).ToByteArray()); // CHECK THIS WORKS
        //     var ECDHi = AesKey.Seed(ECDH_seed);

        //     _logger.LogInformation($"PresignCvk: Returned encrypted gCVKRi for VUID: {vuid}");
        //     return Ok(ECDHi.EncryptStr(gCVKRi.ToByteArray()));
        // }

        // [HttpGet("challenge/{vuid}/{keyId}")]
        // public async Task<ActionResult> Challenge([FromRoute] Guid vuid, [FromRoute] Guid keyId)
        // {
        //     var account = await _managerCvk.GetById(vuid);
        //     if (account == null)
        //     {
        //         _logger.LogInformation("Decryption challenge denied for vuid {0} with keyId {1}: account not found.", vuid, keyId);
        //         return BadRequest($"Denied Challenge for {keyId}");
        //     }

        //     var token = TranToken.Generate(account.CvkiAuth);

        //     var keyPub = await _keyIdManager.GetById(keyId);
        //     if (keyPub == null) {
        //         _logger.LogInformation("Decryption challenge denied for vuid {0} with keyId {1}: keyId not found.", vuid, keyId);
        //         return BadRequest($"Denied Challenge for {keyId}");
        //     }

        //     _logger.LogInformation("Decryption challenge granted for vuid {0} with keyId {1}", vuid, keyId);
        //     var cipher = keyPub.Key.Encrypt(token.GenKey(account.CvkiAuth));
        //     return Ok(new { Token = token.ToString(), Challenge = cipher.ToString() });
        // }

        // [HttpPost("plaintext/{vuid}/{keyId}/{token}/{sign}")]
        // public async Task<ActionResult> Decrypt([FromRoute] Guid vuid, [FromRoute] Guid keyId, [FromBody] string data, string token, string sign)
        // {
        //     var msgErr = $"Denied data decryption belonging to {vuid}";
        //     var account = await _managerCvk.GetById(vuid);

        //     var tran = TranToken.Parse(Convert.FromBase64String(token.DecodeBase64Url()));
        //     if (!tran.Check(account.CvkiAuth)) {
        //         _logger.LogInformation("Decryption denied for vuid {0} with keyId {1}: Invalid token.", vuid, keyId);
        //         return BadRequest(msgErr);
        //     }

        //     var keyPub = await _keyIdManager.GetById(keyId);
        //     if (keyPub == null) {
        //         _logger.LogInformation("Decryption denied for vuid {0} with keyId {1}: keyId not found.", vuid, keyId);
        //         return BadRequest(msgErr);
        //     }

        //     var buffers = GetBytes(data);
        //     if (buffers.Any(bff => !Cipher.CheckAsymmetric(bff, account.CvkPub))) {
        //         _logger.LogInformation("Decryption denied for vuid {0} with keyId {1}: Invalid asymmetric data.", vuid, keyId);
        //         return BadRequest(msgErr);
        //     }

        //     var tags = buffers.Select(bff => Cipher.GetTag(bff)).Distinct().ToList();
        //     var rules = (await _ruleManager.GetSetBy(account.VuId, tags, keyPub.Id)).Where(rl => rl.Eval()).ToList();
        //     if (!tags.All(tag => rules.Where(rule => tag == rule.Tag).Any(rule => rule.IsAllowed))) {
        //         _logger.LogInformation("Decryption denied for vuid {0} with keyId {1}: No rule to allow decryption. Tags: " + string.Join(' ', tags), vuid, keyId);
        //         return BadRequest(msgErr);
        //     }

        //     if (tags.Any(tag => rules.Where(rule => tag == rule.Tag).Any(rule => rule.IsDenied)))
        //     {
        //         _logger.LogInformation("Decryption denied for vuid {0} with keyId {1}: Denied by rule", vuid, keyId);
        //         return BadRequest(msgErr);
        //     }

        //     var bufferSign = Convert.FromBase64String(sign.DecodeBase64Url());
        //     var sessionKey = tran.GenKey(account.CvkiAuth);
        //     if (!Utils.Equals(sessionKey.Hash(buffers.SelectMany(bff => bff).ToArray()), bufferSign)) {
        //         _logger.LogInformation("Decryption denied for vuid {0} with keyId {1}: Invalid symmetric data signature.", vuid, keyId);
        //         return BadRequest(msgErr);
        //     }

        //     var c1s =  buffers.Select(bff => Cipher.GetCipherC1(bff)).ToList();
        //     if (c1s.Any(c1 => !c1.IsValid)) {
        //         _logger.LogInformation("Decryption denied for vuid {0} with keyId {1}: Invalid data point.", vuid, keyId);
        //         return BadRequest(msgErr);
        //     }

        //     _logger.LogInformation("Decryption granted for vuid {0} with keyId {1}", vuid, keyId);
        //     var partials = c1s.Select(c1 => (c1 * account.CVKi).ToByteArray())
        //         .Select(bff => Convert.ToBase64String(sessionKey.Encrypt(bff)));

        //     return Ok(string.Join(Environment.NewLine, partials));
        // }

        // [HttpPost("{vuid}")]
        // public async Task<ActionResult> Confirm([FromRoute] Guid vuid)
        // {
        //     await _managerCvk.Confirm(vuid);
        //     await _ruleManager.ConfirmAll(vuid);
        //     _logger.LogInformation($"Confimed vuid {vuid}", vuid);
        //     return Ok();
        // }
 
        // private byte[] FromBase64(string input)
        // {
        //     return Convert.FromBase64String(input.DecodeBase64Url());
        // }

        // private BigInteger GetBigInteger(string number)
        // {
        //     return new BigInteger(FromBase64(number), true, true);
        // }
        
        // static List<byte[]> GetBytes(string data) {
        //     var line = string.Empty;
        //     var lst = new List<byte[]>();
        //     var rdr = new StringReader(data);
            
        //     while((line = rdr.ReadLine()) != null) {
        //         lst.Add(Convert.FromBase64String(line.Trim()));
        //     }

        //     return lst;
        // }
        
        private async Task<Ed25519Key> GetPubByOrkId(string id)
        {
            var orkNode =_orkManager.GetById(id);
            var orkInfoTask = await orkNode;
            return Ed25519Key.ParsePublic(orkInfoTask.PubKey);
        }

    }
}