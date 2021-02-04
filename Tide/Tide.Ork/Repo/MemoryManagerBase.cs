using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tide.Core;

namespace Tide.Ork.Repo
{
    public abstract class MemoryManagerBase<T> : IManager<T> where T : IGuid, new()
    {
        protected readonly ConcurrentDictionary<Guid, string> _items;

        public MemoryManagerBase()
        {
            _items = new ConcurrentDictionary<Guid, string>();
        }

        public virtual Task Delete(Guid id)
        {
            if (_items.ContainsKey(id))
                _items.Remove(id, out string elm);

            return Task.CompletedTask;
        }

        public virtual Task<bool> Exist(Guid id)
        {
            return Task.FromResult(_items.ContainsKey(id));
        }

        public virtual Task<List<T>> GetAll()
        {
            return Task.FromResult(GetEnumerable().ToList());
        }

        public virtual Task<T> GetById(Guid id)
        {
            if (!_items.ContainsKey(id))
                return Task.FromResult<T>(default(T));

            return Task.FromResult(Map(_items[id]));
        }

        public virtual Task<List<T>> GetByIds(IEnumerable<Guid> ids)
        {
            return Task.FromResult<List<T>>(ids.Where(id => _items.ContainsKey(id)).Select(id => Map(_items[id])).ToList());
        }

        public virtual Task<TideResponse> Add(T entity)
        {
            if (_items.ContainsKey(entity.Id))
                return Task.FromResult(new TideResponse($"The entity [{entity.Id}] already exists"));

            _items[entity.Id] = Map(entity);
            return Task.FromResult(new TideResponse());
        }

        public virtual Task<TideResponse> SetOrUpdate(T entity)
        {
            _items[entity.Id] = Map(entity);
            return Task.FromResult(new TideResponse());
        }

        protected IEnumerable<T> GetEnumerable()
        {
            foreach (KeyValuePair<Guid, string> entry in _items)
            {
                yield return Map(entry.Value);
            }
        }

        protected abstract T Map(string data);

        protected abstract string Map(T entity);
    }
}