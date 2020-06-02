using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Tools;
using Tide.Ork.Models;

namespace Tide.Ork.Classes {
    public class SimulatorFactory : IKeyManagerFactory {
        private readonly IHttpContextAccessor _accessor;
        private readonly AesKey _key;
        private readonly Models.Endpoint _config;

        private HttpRequest Request => _accessor.HttpContext.Request;

        public SimulatorFactory(Settings settings, IHttpContextAccessor accessor) {
            _accessor = accessor;
            _key = AesKey.Parse(settings.Instance.SecretKey);
            _config = settings.Endpoints.Simulator;
        }

        public IKeyManager BuildManager()
        {
            return new SimulatorKeyManager(GetOrkId(), BuildClient(), _key);
        }

        public SimulatorClient BuildClient() {
            return new SimulatorClient(_config.Api, GetOrkId().ToString(), _config.Password);
        }

        private Guid GetOrkId()
        {
            var urlEncoded = Encoding.UTF8.GetBytes(Request.Host.ToString());
            return new Guid(Utils.Hash(urlEncoded).Take(16).ToArray());
        }
    }
}