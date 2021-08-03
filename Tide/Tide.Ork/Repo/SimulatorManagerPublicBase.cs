using System;
using Tide.Core;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public abstract class SimulatorManagerPublicBase<T, U> : SimulatorManagerBase<T> where T : SerializableJson<T, U>, IGuid, new()
    {
        public SimulatorManagerPublicBase(string orkId, SimulatorClient client) : base(orkId, client)
        {
        }

        protected override T Map(string data) => SerializableJson<T, U>.Parse(data);

        protected override string Map(T entity) => entity.ToString();
    }
}
