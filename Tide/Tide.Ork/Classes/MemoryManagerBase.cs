using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption;

namespace Tide.Ork.Classes
{
    public class MemoryManagerBase<T> where T : SerializableByteBase<T>, IGuid, new()
    {
        protected readonly Dictionary<Guid, string> _items;

        public MemoryManagerBase()
        {
            _items = new Dictionary<Guid, string>();
        }

        public Task<bool> Exist(Guid id)
        {
            return Task.FromResult(_items.ContainsKey(id));
        }

        public Task<T> GetByUser(Guid id)
        {
            if (!_items.ContainsKey(id))
                return Task.FromResult<T>(null);

            return Task.FromResult(SerializableByteBase<T>.Parse(_items[id]));
        }

        public Task<TideResponse> SetOrUpdate(T account)
        {
            _items[account.Id] = account.ToString();
            return Task.FromResult(new TideResponse());
        }
    }
}