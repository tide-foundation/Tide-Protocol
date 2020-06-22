using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Ocsp;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Tools;
using Tide.Ork.Models;

namespace Tide.Ork.Classes {
    public class SimulatorFactory : IKeyManagerFactory {
        private readonly AesKey _key;
        private readonly Models.Endpoint _config;
        private readonly string _orkId;

        public SimulatorFactory(Settings settings) {
            _key = AesKey.Parse(settings.Instance.SecretKey);
            _config = settings.Endpoints.Simulator;
            _orkId = settings.Instance.Username;
        }

        public IKeyManager BuildManager()
        {
            return new SimulatorKeyManager(_orkId, BuildClient(), _key);
        }

        public SimulatorClient BuildClient() {
            return new SimulatorClient(_config.Api, _orkId, _config.Password);
        }
    }
}