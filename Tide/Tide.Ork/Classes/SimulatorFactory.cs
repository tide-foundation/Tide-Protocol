using System;
using Tide.Core;
using Tide.Encryption.AesMAC;
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

        public IKeyManager BuildManager() => new SimulatorKeyManager(_orkId, BuildClient(), _key);

        public IManager<CvkVault> BuildManagerCvk() => new SimulatorManagerBase<CvkVault>(_orkId, BuildClient(), _key);

        public SimulatorClient BuildClient() => new SimulatorClient(_config.Api, _orkId, _config.Password);

        public IManager<KeyIdVault> BuildKeyIdManager() => new SimulatorManagerBase<KeyIdVault>(_orkId, BuildClient(), _key);

        public IRuleManager BuildRuleManager() => new SimulatorRuleManager(_orkId, BuildClient(), _key);
    }
}