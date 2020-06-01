using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Library;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Classes {
    public class MemoryKeyManager : IKeyManager {
        private readonly Dictionary<Guid, KeyVault> _items;

        public MemoryKeyManager() {
          
            _items = new Dictionary<Guid, KeyVault>();
        }

        public Task<bool> Exist(Guid user) {
            return Task.FromResult(_items.ContainsKey(user));
        }

        public Task<BigInteger> GetAuthShare(Guid user) {
            return Task.FromResult(_items.ContainsKey(user) ? _items[user].AuthShare : BigInteger.Zero);
        }

        public Task<AesKey> GetSecret(Guid user) {
            return Task.FromResult(_items.ContainsKey(user) ? _items[user].Secret : null);
        }

        public Task<KeyVault> GetByUser(Guid user) {
            return Task.FromResult(_items.ContainsKey(user) ? _items[user] : null);
        }

        public Task SetOrUpdateKey(Guid user, BigInteger authShare, BigInteger keyShare, AesKey secret) {
            _items[user] = new KeyVault { User = user, AuthShare = authShare, KeyShare = keyShare, Secret = secret};
            return Task.CompletedTask;
        }
    }
}