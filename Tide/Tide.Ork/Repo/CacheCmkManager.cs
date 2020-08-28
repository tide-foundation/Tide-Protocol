using System;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tide.Core;

namespace Tide.Ork.Repo
{
    public class CacheCmkManager : CacheManager<CmkVault>, ICmkManager
    {
        private readonly ICmkManager _manager;

        public CacheCmkManager(IMemoryCache cache, ICmkManager manager) : base(cache, manager)
        {
            _manager = manager;
        }

        public async Task<BigInteger> GetPrism(Guid user)
        {
            var vault = await GetById(user);

            return vault != null ? vault.Prismi
                : await _manager.GetPrism(user);
        }
    }
}