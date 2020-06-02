namespace Tide.Ork.Classes {
    public class MemoryFactory : IKeyManagerFactory {
        private static readonly MemoryKeyManager _manager = new MemoryKeyManager();

        public IKeyManager BuildManager() {
            return _manager;
        }
    }
}