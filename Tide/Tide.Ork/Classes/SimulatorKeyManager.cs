using System;
using System.Numerics;
using System.Threading.Tasks;
using Library;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Classes
{
    public class SimulatorKeyManager : IKeyManager
    {
        private readonly Guid _orkId;
        private readonly SimulatorClient _client;
        private readonly AesKey _key;

        public SimulatorKeyManager(Guid orkId, SimulatorClient client, AesKey key)
        {
            _orkId = orkId;
            _client = client;
            _key = key;
        }

        public async Task<bool> Exist(Guid user)
        {
            return (await GetByUser(user)) == null;
        }

        public async Task<BigInteger> GetAuthShare(Guid user)
        {
            return (await GetByUser(user)).AuthShare;
        }

        public async Task<AesKey> GetSecret(Guid user)
        {
            return (await GetByUser(user)).Secret;
        }

        public async Task<KeyVault> GetByUser(Guid user)
        {
            var cipher = await _client.GetVault(_orkId.ToString(), user.ToString());
            if (string.IsNullOrEmpty(cipher))
                return null;

            return KeyVault.Parse(_key.Decrypt(cipher));
        }

        public async Task SetOrUpdateKey(Guid user, BigInteger authShare, BigInteger keyShare, AesKey secret)
        {
            var vault = new KeyVault
            {
                User = user,
                AuthShare = authShare,
                KeyShare = keyShare,
                Secret = secret,
            };
            await _client.PostVault(_orkId.ToString(), user.ToString(), _key.EncryptStr(vault));
        }
    }
}