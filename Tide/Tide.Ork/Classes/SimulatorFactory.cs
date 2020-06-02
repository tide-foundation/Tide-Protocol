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
        private readonly Settings _settings;

        public SimulatorFactory(Settings settings, IHttpContextAccessor accessor) {
            _settings = settings;
            _accessor = accessor;
        }

        private HttpRequest Request => _accessor.HttpContext.Request;

        public IKeyManager BuildManager() {
            var orkId = new Guid(Utils.Hash(Encoding.UTF8.GetBytes(Request.Host.ToString())).Take(16).ToArray());
            var key = AesKey.Parse(_settings.Instance.SecretKey);
            var client = BuildClient(orkId.ToString());

            return new SimulatorKeyManager(orkId, client, key);
        }

        public SimulatorClient BuildClient(string orkId) {
            return new SimulatorClient(_settings.Endpoints.Simulator.Api, orkId, _settings.Endpoints.Simulator.Password);
        }
    }
}