using Microsoft.Extensions.Caching.Memory;
using Tide.Core;

namespace Tide.Ork.Repo {
    public class MemoryFactory : IKeyManagerFactory {
        private static readonly ICmkManager _manager = new MemoryCmkManager();
        private static readonly ICvkManager _managerCvk = new MemoryCvkManager();
        private static readonly IKeyIdManager _keyIdManager = new MemoryKeyIdManager();
        private static readonly IRuleManager _memoryRuleManager = new MemoryRuleManager();
        private static readonly IDnsManager _memoryDnsManager = new MemoryDnsManager();
        private readonly IMemoryCache _cache;

        public MemoryFactory(IMemoryCache cache)
        {
            _cache = cache;
        }

        public ICmkManager BuildCmkManager() => _manager;//new CacheCmkManager(_cache, _manager);

        public ICvkManager BuildManagerCvk() => _managerCvk; //new CacheCvkManager(_cache, _managerCvk);

        public IKeyIdManager BuildKeyIdManager() => _keyIdManager;

        public IRuleManager BuildRuleManager() => _memoryRuleManager; //new CacheRuleManager(_cache, _memoryRuleManager);

        public IDnsManager BuildDnsManager() => _memoryDnsManager;
    }
}