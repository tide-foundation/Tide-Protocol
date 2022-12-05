using System;
using System.Threading;
using LazyCache;
using LazyCache.Providers;
using Microsoft.Extensions.Caching.Memory;
namespace Tide.Ork.Classes
{
    public class CachingManager : IDisposable
    {
        private readonly CachingService Cache;

        public CachingManager(int size = 1024)
        {
            Cache = new CachingService(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = size
            })));
        }

        public string AddorGetCache(string id, string ri)
        {
            return Cache.GetOrAdd(id, () =>{
                Console.WriteLine($"{DateTime.UtcNow}: Fetching or store from service");

                var item = ri;
                return item;
            }, BuildPolicy());           
        }


        private MemoryCacheEntryOptions BuildPolicy() => (new MemoryCacheEntryOptions())
            .SetSize(1)
            .SetPriority(CacheItemPriority.NeverRemove)
            //.SetSlidingExpiration(DateTimeOffset.Now.AddSeconds(1200))
            .SetAbsoluteExpiration(DateTimeOffset.Now.AddSeconds(1200));

        public void Dispose() => Cache.CacheProvider.Dispose();

        public void Remove(string id) => Cache.Remove(id);

    

    }
}