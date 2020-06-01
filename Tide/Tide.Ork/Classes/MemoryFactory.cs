using System;

namespace Tide.Ork.Classes
{
    public class MemoryFactory : IKeyManagerFactory
    {
        static readonly MemoryKeyManager _manager = new MemoryKeyManager();

        public MemoryFactory() { }

        public IKeyManager BuildManager()
        {
            return _manager;
        }
    }
}
