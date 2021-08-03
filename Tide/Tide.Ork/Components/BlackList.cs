using System;
using Microsoft.Extensions.Caching.Memory;
namespace Tide.Ork.Classes
{
    public class BlackList : IDisposable
    {
        private readonly TimeSpan _lapse;
        public MemoryCache Cache { get; }

        public BlackList(int size = 1024) : this(TimeSpan.FromMinutes(5), size) { }

        public BlackList(TimeSpan lapse, int size = 1024)
        {
            _lapse = lapse;
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = size
            });
        }

        public bool Add(string id) => Cache.Set<bool>(id, true, BuildPolicy());

        public bool Add(string id, TimeSpan lapse) => Cache.Set<bool>(id, true, BuildPolicy(lapse));

        public bool Exist(string id) => Cache.TryGetValue<bool>(id, out var element) && element;

        private MemoryCacheEntryOptions BuildPolicy(TimeSpan? lapse = null) => (new MemoryCacheEntryOptions())
            .SetAbsoluteExpiration(lapse ?? _lapse).SetSize(1);

        public void Dispose() => Cache.Dispose();
    }
}