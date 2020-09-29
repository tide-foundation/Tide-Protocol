using System;
using System.Numerics;
using System.Threading.Tasks;
using Tide.Core;

namespace Tide.Ork.Repo
{
    public class MemoryCmkManager : MemoryManagerBase<CmkVault>, ICmkManager
    {
        public Task Confirm(Guid id) => Task.CompletedTask; //throw new NotImplementedException("Do not invoke confirm in memory manager");

        public async Task<BigInteger> GetPrism(Guid user)
        {
            var vault = await this.GetById(user);
            return vault != null ? vault.Prismi : BigInteger.Zero;
        }
    }
}