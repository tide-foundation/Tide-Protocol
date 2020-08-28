using System;
using Microsoft.Extensions.Caching.Memory;
using Tide.Core;

namespace Tide.Ork.Repo
{
    public class CacheCvkManager : CacheManager<CvkVault>, ICvkManager
    {
        public CacheCvkManager(IMemoryCache cache, ICvkManager manager) : base(cache, manager)
        {
        }
    }
}