using System;
using Microsoft.Extensions.Caching.Memory;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ecc;
using Tide.Ork.Classes;
using Tide.Ork.Models;

namespace Tide.Ork.Repo {
    public class SimulatorFactory : IKeyManagerFactory {
        private readonly AesKey _key;
        private readonly Models.Endpoint _config;
        private readonly string _orkId;
        private readonly IMemoryCache _cache;
        private readonly C25519Key _private;

        public SimulatorFactory(IMemoryCache cache, Settings settings) {
            _key = settings.Instance.GetSecretKey();
            _private = settings.Instance.GetPrivateKey();
            _config = settings.Endpoints.Simulator;
            _orkId = settings.Instance.Username;
            _cache = cache;
           
        }

        public ICmkManager BuildCmkManager() => new SimulatorCmkManager(_orkId, BuildClient(), _key); //new CacheCmkManager(_cache, new SimulatorCmkManager(_orkId, BuildClient(), _key));

        public ICvkManager BuildManagerCvk() => new SimulatorCvkManager(_orkId, BuildClient(), _key); //new CacheCvkManager(_cache, new SimulatorCvkManager(_orkId, BuildClient(), _key));

        public SimulatorClient BuildClient() => new SimulatorClient(_config.Api, _orkId, _private);

        public IKeyIdManager BuildKeyIdManager() => new SimulatorKeyIdManager(_orkId, BuildClient(), _key);

        public IRuleManager BuildRuleManager() => new SimulatorRuleManager(_orkId, BuildClient(), _key); //new CacheRuleManager(_cache, new SimulatorRuleManager(_orkId, BuildClient(), _key));
    }
}