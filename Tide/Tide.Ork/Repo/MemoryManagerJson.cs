using System;
using Tide.Core;

namespace Tide.Ork.Repo
{
    public abstract class MemoryManagerJson<TClass, TDto> : MemoryManagerBase<TClass> where TClass : SerializableJson<TClass, TDto>, IGuid, new()
    {
        protected override TClass Map(string data) => SerializableJson<TClass, TDto>.Parse(data);

        protected override string Map(TClass entity) => entity.ToString();
    }
}