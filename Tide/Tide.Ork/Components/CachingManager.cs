using System;
using System.Threading;
using LazyCache;
using LazyCache.Providers;
using Microsoft.Extensions.Caching.Memory;
namespace Tide.Ork.Classes
{
    public class CachingManager : IDisposable
    {
        private readonly CachingService _cache;

        public CachingManager()
        {
            _cache = new CachingService();
        }

        public string AddorGetCache(string id, string entry)
        {
            return _cache.GetOrAdd(id, () =>{
                Console.WriteLine($"{DateTime.UtcNow}: Fetching or store from service");

                var item = entry;
                return item;
            }, BuildPolicy());           
        }


        private MemoryCacheEntryOptions BuildPolicy() => (new MemoryCacheEntryOptions())
            .SetPriority(CacheItemPriority.NeverRemove)
            //.SetSlidingExpiration(DateTimeOffset.Now.AddSeconds(1200))
            .SetAbsoluteExpiration(DateTimeOffset.Now.AddSeconds(1200));

        public void Dispose() => _cache.CacheProvider.Dispose();

        public void Remove(string id) => _cache.Remove(id);

    
    }
}