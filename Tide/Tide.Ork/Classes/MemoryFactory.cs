using Tide.Core;

namespace Tide.Ork.Classes {
    public class MemoryFactory : IKeyManagerFactory {
        private static readonly MemoryKeyManager _manager = new MemoryKeyManager();
        private static readonly MemoryCvkManager _managerCvk = new MemoryCvkManager();

        public IKeyManager BuildManager() {
            return _manager;
        }

        public IManager<CvkVault> BuildManagerCvk()
        {
            return _managerCvk;
        }
    }
}