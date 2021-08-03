using System;
using System.Threading;
using LazyCache;
using LazyCache.Providers;
using Microsoft.Extensions.Caching.Memory;
namespace Tide.Ork.Classes
{
    public class ThrottlingManager : IDisposable
    {
        private readonly CachingService Cache;

        public ushort Allow { get; set; }

        public TimeSpan Lapse { get; set; }

        public ThrottlingManager(int size = 1024)
        {
            Allow = 10;
            Lapse = TimeSpan.FromMinutes(1);

            Cache = new CachingService(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = size
            })));
        }

        public bool Throttle(string id)
        {
            var req = Cache.GetOrAdd<RequestCache>(id, () => new RequestCache(), BuildPolicy());

            Interlocked.Increment(ref req.Times);
            var times = (int) Math.Ceiling((DateTime.UtcNow - req.Start).Ticks / (double) Lapse.Ticks);

            return req.Times > Allow * times;
        }

        private TimeSpan ExpirationDistribution() {
            if (Lapse.Ticks <= TimeSpan.TicksPerSecond || Allow <= 1)
                return Lapse;

            var quantum = (Lapse.Ticks * Allow - TimeSpan.TicksPerSecond) / 100;
            var total = (DateTime.UtcNow.Millisecond % 100) + 1;
            return TimeSpan.FromTicks(quantum * total + TimeSpan.TicksPerSecond);
        }

        private MemoryCacheEntryOptions BuildPolicy() => (new MemoryCacheEntryOptions())
            .SetSize(1).SetSlidingExpiration(ExpirationDistribution());

        public void Dispose() => Cache.CacheProvider.Dispose();

        public void Remove(string id) => Cache.Remove(id);

        private class RequestCache
        {
            public int Times = 0;
            public DateTime Start = DateTime.UtcNow;
        }

    }
}