using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tide.Core;

namespace Tide.Ork.Repo
{
    public class CacheManager<T> where T : IGuid, new()
    {
        protected readonly TimeSpan life = TimeSpan.FromSeconds(90);
        protected readonly IMemoryCache Cache;
        private readonly IManager<T> Manager;

        public CacheManager(IMemoryCache cache, IManager<T> manager)
        {
            this.Cache = cache;
            this.Manager = manager;
        }

        //TODO: SetOrUpdate must be separated into two methods
        public async Task<TideResponse> SetOrUpdate(T entity)
        {
            if (await Manager.Exist(entity.Id))
                return await Manager.SetOrUpdate(entity);
            
            Cache.Set(entity.Id, entity, life);
            return new TideResponse(true);
        }

        public async Task Confirm(Guid id)
        {
            var data = Cache.Get<T>(id);
            if (data == null) return;
            
            await Manager.SetOrUpdate(data);
            Cache.Remove(id);
        }

        public Task Delete(Guid id)
        {
            var data = Cache.Get<T>(id);
            if (data == null) return Manager.Delete(id);

            Cache.Remove(id);
            return Task.CompletedTask;
        }

        public Task<bool> Exist(Guid id)
        {
            return Cache.Get(id) != null ? Task.FromResult(true) : Manager.Exist(id);
        }

        public Task<List<T>> GetAll()
        {
            return Manager.GetAll();
        }

        public async Task<T> GetById(Guid id)
        {
            return Cache.Get<T>(id) ?? await Manager.GetById(id);
        }

        public Task Rollback(Guid id)
        {
            if (Cache.Get(id) != null) Cache.Remove(id);
            
            return Task.CompletedTask;
        }

        protected IEnumerable<T> GetEnumerable()
        {
            foreach (var id in GetGuids())
            {
                var item = Cache.Get(id);
                if (item != null && item.GetType() == typeof(T))
                    yield return (T) item;
            }
        }

        protected IEnumerable<Guid> GetGuids() {
            var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
            var collection = field.GetValue(Cache) as ICollection;
            if (collection == null)
                yield break;

            foreach (var item in collection)
            {
                var id = item.GetType().GetProperty("Key").GetValue(item);
                if (id != null && id.GetType() == typeof(Guid))
                    yield return (Guid)id;
            }
        }
    }
}