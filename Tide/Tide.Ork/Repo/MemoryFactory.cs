using Tide.Core;

namespace Tide.Ork.Repo {
    public class MemoryFactory : IKeyManagerFactory {
        private static readonly ICmkManager _manager = new MemoryCmkManager();
        private static readonly ICvkManager _managerCvk = new MemoryCvkManager();
        private static readonly IKeyIdManager _keyIdManager = new MemoryKeyIdManager();
        private static readonly IRuleManager _memoryRuleManager = new MemoryRuleManager();
        
        public ICmkManager BuildCmkManager() {
            return _manager;
        }

        public ICvkManager BuildManagerCvk()
        {
            return _managerCvk;
        }

        public IKeyIdManager BuildKeyIdManager()
        {
            return _keyIdManager;
        }

        public IRuleManager BuildRuleManager()
        {
            return _memoryRuleManager;
        }
    }
}