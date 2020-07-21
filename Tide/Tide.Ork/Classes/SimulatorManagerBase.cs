using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Classes {
    public class SimulatorManagerBase<T> : IManager<T> where T : SerializableByteBase<T>, IGuid, new()
    {
        protected readonly SimulatorClient _client;
        protected readonly AesKey _key;
        protected readonly string _orkId;

        public SimulatorManagerBase(string orkId, SimulatorClient client, AesKey key) {
            _orkId = orkId;
            _client = client;
            _key = key;
        }

        public async Task<bool> Exist(Guid id) {
            return await GetById(id) == null;
        }

        //TODO: Ask Matt for help
        public Task<List<T>> GetAll()
        {
            throw new NotImplementedException();
        }

        //TODO: Ask Matt for help
        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetById(Guid id) {
            var response = await _client.GetVault(_orkId, id.ToString());
            if (!response.Success) return null;

            return SerializableByteBase<T>.Parse(_key.Decrypt((string)response.Content));
        }

        public async Task<TideResponse> SetOrUpdate(T entity)
        {
            return await _client.PostVault(_orkId, entity.Id.ToString(), _key.EncryptStr(entity));
        }
    }
}