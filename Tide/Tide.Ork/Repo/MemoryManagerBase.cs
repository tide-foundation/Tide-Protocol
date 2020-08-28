using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption;

namespace Tide.Ork.Repo
{
    public class MemoryManagerBase<T> : IManager<T> where T : SerializableByteBase<T>, IGuid, new()
    {
        protected readonly ConcurrentDictionary<Guid, string> _items;

        public MemoryManagerBase()
        {
            _items = new ConcurrentDictionary<Guid, string>();
        }

        public Task Delete(Guid id)
        {
            if (_items.ContainsKey(id))
                _items.Remove(id, out string elm);

            return Task.CompletedTask;
        }

        public Task<bool> Exist(Guid id)
        {
            return Task.FromResult(_items.ContainsKey(id));
        }

        public Task<List<T>> GetAll()
        {
            return Task.FromResult(GetEnumerable().ToList());
        }

        public Task<T> GetById(Guid id)
        {
            if (!_items.ContainsKey(id))
                return Task.FromResult<T>(null);

            return Task.FromResult(SerializableByteBase<T>.Parse(_items[id]));
        }

        public Task<TideResponse> SetOrUpdate(T entity)
        {
            _items[entity.Id] = entity.ToString();
            return Task.FromResult(new TideResponse());
        }

        protected IEnumerable<T> GetEnumerable()
        {
            foreach (KeyValuePair<Guid, string> entry in _items)
            {
                yield return SerializableByteBase<T>.Parse(entry.Value);
            }
        }
    }
}