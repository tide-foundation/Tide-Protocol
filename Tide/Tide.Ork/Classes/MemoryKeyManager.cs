using System;
using System.Numerics;
using System.Threading.Tasks;
using Tide.Core;

namespace Tide.Ork.Classes
{
    public class MemoryKeyManager : MemoryManagerBase<KeyVault>, IKeyManager
    {
        public async Task<BigInteger> GetAuthShare(Guid user)
        {
            var vault = await this.GetById(user);
            return vault != null ? vault.AuthShare : BigInteger.Zero;
        }
    }
}