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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ecc;
using Tide.Encryption.Tools;
using Tide.Ork.Classes;

namespace Tide.Ork.Controllers {
    [ApiController]
    [Route("api/dauth")]
    public class DAuthController : ControllerBase {
        private const long _window = TimeSpan.TicksPerSecond * 15;
        private readonly ILogger _logger;
        private readonly IKeyManager _manager;

        public DAuthController(IKeyManagerFactory factory, ILogger<DAuthController> logger) {
            _manager = factory.BuildManager();
            _logger = logger;
        }

        [HttpGet("{user}/share/{pass}")]
        public async Task<ActionResult> GetShare([FromRoute] string user, [FromRoute] string pass) {
            var g = C25519Point.From(Convert.FromBase64String(pass.DecodeBase64Url()));
            if (!g.IsValid) {
                return BadRequest();
            }

            var s = await _manager.GetAuthShare(GetUserId(user));
            var gs = g * s;

            _logger.LogInformation($"Login attempt for {user}", user, pass);
            return Ok(Convert.ToBase64String(gs.ToByteArray()));
        }

        [HttpGet("{user}/signin/{ticks}/{sign}")]
        public async Task<ActionResult> SignIn([FromRoute] string user, [FromRoute] string ticks, [FromRoute] string sign) {
            var body = FromBase64(user).Concat(FromBase64(ticks)).ToArray();
            var secret = await _manager.GetByUser(GetUserId(user));

            var check = secret.Secret.Hash(body);
            if (!Utils.Equals(check, FromBase64(sign))) {
                _logger.LogInformation($"Unsuccessful login for {user}", user, ticks, sign);
                return BadRequest();
            }

            var signedTime = DateTime.FromBinary((long) GetBigInteger(ticks));
            if (signedTime < DateTime.UtcNow.AddTicks(-_window)
                || signedTime > DateTime.UtcNow.AddTicks(_window)) {
                _logger.LogInformation($"Unsuccessful login for {user} due to invalid ticks", user, ticks, sign);
                return BadRequest();
            }

            _logger.LogInformation($"Successful login for {user}", user, ticks, sign);
            return Ok(secret.Secret.EncryptStr(secret.KeyShare.ToByteArray(true, true)));
        }

        [HttpPost("{user}/signup/{authShare}/{keyShare}/{secret}")]
        public Task SignUp([FromRoute] string user, [FromRoute] string authShare, [FromRoute] string keyShare, [FromRoute] string secret) {
            _logger.LogInformation($"New registration for {user}", user);
            return _manager.SetOrUpdateKey(GetUserId(user), GetBigInteger(authShare), GetBigInteger(keyShare), AesKey.Parse(FromBase64(secret)));
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