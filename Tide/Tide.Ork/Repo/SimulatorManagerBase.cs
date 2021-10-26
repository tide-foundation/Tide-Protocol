using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public abstract class SimulatorManagerBase<T> where T : IGuid, new()
    {
        protected readonly SimulatorClient _client;
        protected readonly string _orkId;
        protected abstract string TableName { get; }
        protected abstract string Contract { get; }

        public SimulatorManagerBase(string orkId, SimulatorClient client) {
            _orkId = orkId;
            _client = client;
        }

        public async Task<bool> Exist(Guid id) => await GetById(id) != null;

        public async Task<List<T>> GetAll()
        {
            var response = await _client.Get(Contract, TableName, _orkId);
            return response.Select(Map).ToList();
        }

        public async Task<T> GetById(Guid id) {
            var response = await _client.Get(Contract, TableName, _orkId, id.ToString());
            if (string.IsNullOrEmpty(response))
                return default(T);

            return Map(response);
        }

        public async Task<List<T>> GetByIds(IEnumerable<Guid> ids)
        {
            var response = await _client.Get(Contract, TableName, _orkId, ids.Select(id => id.ToString()));
            return response.Select(Map).ToList();
        }

        public async Task<TideResponse> Add(T entity)
        {
            if (await Exist(entity.Id))
                return new TideResponse($"The entity [{entity.Id}] already exists");
            
            return await SetOrUpdate(entity);
        }

        public async Task<TideResponse> SetOrUpdate(T entity)
        {
            var result = await _client.Post(Contract, TableName, _orkId, entity.Id.ToString(), Map(entity));
            if(result.success) return new TideResponse(true);
            return new TideResponse(false,null,result.error);
        }

        public Task<bool> Delete(Guid id) {
            return _client.Delete(Contract, TableName, _orkId, id.ToString());
        }

        protected abstract T Map(string data);

        protected abstract string Map(T entity);
    }
}
