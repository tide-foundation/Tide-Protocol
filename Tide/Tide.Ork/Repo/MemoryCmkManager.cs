using System;
using System.Numerics;
using System.Threading.Tasks;
using Tide.Core;

namespace Tide.Ork.Repo
{
    public class MemoryCmkManager : MemoryManagerBase<CmkVault>, ICmkManager
    {
        public async Task<BigInteger> GetPrism(Guid user)
        {
            var vault = await this.GetById(user);
            return vault != null ? vault.Prismi : BigInteger.Zero;
        }
    }
}