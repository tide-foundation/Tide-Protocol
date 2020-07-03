using System;
using System.Numerics;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Classes {
    //TODO: This has to be refactored with SimulatorKeyManager
    public class SimulatorCvkManager : ICvkManager {
        private readonly SimulatorClient _client;
        private readonly AesKey _key;
        private readonly string _orkId;

        public SimulatorCvkManager(string orkId, SimulatorClient client, AesKey key) {
            _orkId = orkId;
            _client = client;
            _key = key;
        }

        public async Task<bool> Exist(Guid user) {
            return await GetByUser(user) == null;
        }

        public async Task<BigInteger> GetShare(Guid userId) {
            var user = await GetByUser(userId);
            if (user == null) return BigInteger.Zero;
            return user.CVKi;
        }

        public async Task<AesKey> GetAuth(Guid userId) {
            var user = await GetByUser(userId);
            return user?.CvkAuth;
        }

        public async Task<CvkVault> GetByUser(Guid userId) {
            var response = await _client.GetVault(_orkId, userId.ToString());
            if (!response.Success) return null;

            return CvkVault.Parse(_key.Decrypt((string)response.Content));
        }

        public async Task<TideResponse> SetOrUpdate(CvkVault account)
        {
            return await _client.PostVault(_orkId, account.User.ToString(), _key.EncryptStr(account));
        }
    }
}