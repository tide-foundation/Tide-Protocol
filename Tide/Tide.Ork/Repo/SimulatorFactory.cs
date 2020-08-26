using System;
using Tide.Encryption.AesMAC;
using Tide.Ork.Classes;
using Tide.Ork.Models;

namespace Tide.Ork.Repo {
    public class SimulatorFactory : IKeyManagerFactory {
        private readonly AesKey _key;
        private readonly Models.Endpoint _config;
        private readonly string _orkId;

        public SimulatorFactory(Settings settings) {
            _key = settings.Instance.GetSecretKey();
            _config = settings.Endpoints.Simulator;
            _orkId = settings.Instance.Username;
        }

        public ICmkManager BuildCmkManager() => new SimulatorCmkManager(_orkId, BuildClient(), _key);

        public ICvkManager BuildManagerCvk() => new SimulatorCvkManager(_orkId, BuildClient(), _key);

        public SimulatorClient BuildClient() => new SimulatorClient(_config.Api, _orkId, _config.Password);

        public IKeyIdManager BuildKeyIdManager() => new SimulatorKeyIdManager(_orkId, BuildClient(), _key);

        public IRuleManager BuildRuleManager() => new SimulatorRuleManager(_orkId, BuildClient(), _key);
    }
}