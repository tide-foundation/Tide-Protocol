using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption;
using Tide.Encryption.AesMAC;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public abstract class SimulatorManagerBase<T> where T : SerializableByteBase<T>, IGuid, new()
    {
        protected readonly SimulatorClient _client;
        protected readonly AesKey _key;
        protected readonly string _orkId;

        protected virtual bool IsEncrypted => true;

        protected abstract string TableName { get; }
        
        public SimulatorManagerBase(string orkId, SimulatorClient client, AesKey key) {
            _orkId = orkId;
            _client = client;
            _key = key;
        }

        public async Task<bool> Exist(Guid id) {
            return await GetById(id) == null;
        }

        public async Task<List<T>> GetAll()
        {
            var response = await _client.Get("Simulator", TableName, _orkId);
            return response.Select(Map).ToList();
        }

        public async Task<T> GetById(Guid id) {
            var response = await _client.Get("Simulator", TableName, _orkId, id.ToString());
            if (string.IsNullOrEmpty(response))
                return null;

            return Map(response);
        }

        public async Task<TideResponse> SetOrUpdate(T entity)
        {
            var ok = await _client.Post("Simulator", TableName, _orkId, entity.Id.ToString(), Map(entity));
            return new TideResponse() { Success = ok };
        }

        public Task Delete(Guid id) => throw new NotImplementedException();

        private T Map(string data) => !IsEncrypted ? SerializableByteBase<T>.Parse(data.Replace("\"", ""))
            : SerializableByteBase<T>.Parse(_key.Decrypt(data.Replace("\"", "")));

        private string Map(T entity) => !IsEncrypted ? entity.ToString() : _key.EncryptStr(entity);
    }
}
