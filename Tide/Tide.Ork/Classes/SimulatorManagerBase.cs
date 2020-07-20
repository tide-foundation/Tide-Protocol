using System;
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

        public async Task<bool> Exist(Guid user) {
            return await GetByUser(user) == null;
        }

        public async Task<T> GetByUser(Guid id) {
            var response = await _client.GetVault(_orkId, id.ToString());
            if (!response.Success) return null;

            return SerializableByteBase<T>.Parse(_key.Decrypt((string)response.Content));
        }

        public async Task<TideResponse> SetOrUpdate(T account)
        {
            return await _client.PostVault(_orkId, account.Id.ToString(), _key.EncryptStr(account));
        }
    }
}