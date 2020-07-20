using Tide.Core;

namespace Tide.Ork.Classes {
    public class MemoryFactory : IKeyManagerFactory {
        private static readonly MemoryKeyManager _manager = new MemoryKeyManager();
        private static readonly IManager<CvkVault> _managerCvk = new MemoryManagerBase<CvkVault>();
        private static readonly IManager<KeyIdVault> _keyIdManager = new MemoryManagerBase<KeyIdVault>();
        private static readonly IManager<RuleVault> _memoryRuleManager = new MemoryManagerBase<RuleVault>();
        
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

        public IManager<RuleVault> BuildRuleManager()
        {
            return _memoryRuleManager;
        }
    }
}