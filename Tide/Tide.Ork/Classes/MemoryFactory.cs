using Tide.Core;

namespace Tide.Ork.Classes {
    public class MemoryFactory : IKeyManagerFactory {
        private static readonly MemoryKeyManager _manager = new MemoryKeyManager();
        private static readonly IManager<CvkVault> _managerCvk = new MemoryManagerBase<CvkVault>();
        private static readonly IManager<KeyIdVault> _keyIdManager = new MemoryManagerBase<KeyIdVault>();
        private static readonly IRuleManager _memoryRuleManager = new MemoryRuleManager();
        
        public IKeyManager BuildManager() {
            return _manager;
        }

        public IManager<CvkVault> BuildManagerCvk()
        {
            return _managerCvk;
        }

        public IManager<KeyIdVault> BuildKeyIdManager()
        {
            return _keyIdManager;
        }

        public IRuleManager BuildRuleManager()
        {
            return _memoryRuleManager;
        }
    }
}