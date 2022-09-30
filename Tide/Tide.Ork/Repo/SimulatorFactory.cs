using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ed;
using Tide.Ork.Classes;
using Tide.Ork.Models;

namespace Tide.Ork.Repo {
    public class SimulatorFactory : IKeyManagerFactory {
        private readonly AesKey _key;
        private readonly Models.Endpoint _config;
        private readonly string _orkId;
        private readonly IMemoryCache _cache;
        private readonly HttpContext _context;
        private readonly Ed25519Key _private;

        private bool IsCache => !_context.Request.Headers.TryGetValue("cache", out StringValues keys) ? false
            : (keys.FirstOrDefault() ?? string.Empty).Trim().ToLower() == "true";

        public SimulatorFactory(IMemoryCache cache, IHttpContextAccessor context, Settings settings) {
            _key = settings.Instance.GetSecretKey();
            _private = settings.Instance.GetPrivateKey();
            _config = settings.Endpoints.Simulator;
            _orkId = settings.Instance.Username;
            _cache = cache;
            _context = context.HttpContext;
        }

        public ICmkManager BuildCmkManager() {
            ICmkManager repo = new SimulatorCmkManager(_orkId, BuildClient(), _key);
            return IsCache ? new CacheCmkManager(_cache, repo) : repo;
        }

        public ICvkManager BuildManagerCvk() {
            ICvkManager repo = new SimulatorCvkManager(_orkId, BuildClient(), _key);
            return IsCache ? new CacheCvkManager(_cache, repo) : repo;
        }

        public SimulatorClient BuildClient() => new SimulatorClient(_config.Api, _orkId, _private);

        public IKeyIdManager BuildKeyIdManager() => new SimulatorKeyIdManager(_orkId, BuildClient(), _key);

        public IRuleManager BuildRuleManager() {
            IRuleManager repo = new SimulatorRuleManager(_orkId, BuildClient(), _key);
            return IsCache ? new CacheRuleManager(_cache, repo) : repo;
        }

        public IDnsManager BuildDnsManager()
        {
            return new SimulatorDnsManager(BuildClient());
        }
    }
}