using System;
using System.Numerics;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Classes {
    public class SimulatorKeyManager : IKeyManager {
        private readonly SimulatorClient _client;
        private readonly AesKey _key;
        private readonly string _orkId;

        public SimulatorKeyManager(string orkId, SimulatorClient client, AesKey key) {
            _orkId = orkId;
            _client = client;
            _key = key;
        }

        public async Task<bool> Exist(Guid user) {
            return await GetByUser(user) == null;
        }

        public async Task<BigInteger> GetAuthShare(Guid user) {
            return (await GetByUser(user)).AuthShare;
        }

        public async Task<AesKey> GetSecret(Guid user) {
            return (await GetByUser(user)).Secret;
        }

        public async Task<KeyVault> GetByUser(Guid user) {
            var cipher = await _client.GetVault(_orkId, user.ToString());
            if (string.IsNullOrEmpty(cipher)) return null;

            return KeyVault.Parse(_key.Decrypt(cipher));
        }

        public async Task<TideResponse> SetOrUpdate(KeyVault account)
        {
            return await _client.PostVault(_orkId, account.User.ToString(), _key.EncryptStr(account));
        }
    }
}