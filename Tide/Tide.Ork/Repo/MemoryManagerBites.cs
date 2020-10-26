using System;
using Tide.Core;
using Tide.Encryption;

namespace Tide.Ork.Repo
{
    public abstract class MemoryManagerBites<T> : MemoryManagerBase<T> where T : SerializableByteBase<T>, IGuid, new()
    {
        protected override T Map(string data) => SerializableByteBase<T>.Parse(data);

        protected override string Map(T entity) => entity.ToString();
    }
}