using System;
using System.Collections.Generic;
using System.Numerics;
using Tide.Encryption.AesMAC;
using Tide.Ork.Models;

namespace Tide.Ork.Classes {
    public class MemoryKeyManager : IKeyManager {
        private readonly Dictionary<Guid, KeyVault> _items;

        public MemoryKeyManager() {
            _items = new Dictionary<Guid, KeyVault>();
        }

        public bool Exist(Guid user) {
            return _items.ContainsKey(user);
        }

        public BigInteger GetAuthShare(Guid user) {
            return Exist(user) ? _items[user].AuthShare : BigInteger.Zero;
        }

        public AesKey GetSecret(Guid user) {
            return Exist(user) ? _items[user].Secret : null;
        }

        public KeyVault GetByUser(Guid user) {
            return Exist(user) ? _items[user] : null;
        }

        public void SetOrUpdateKey(Guid user, BigInteger authShare, BigInteger keyShare, AesKey secret) {
            _items[user] = new KeyVault {User = user, AuthShare = authShare, KeyShare = keyShare, Secret = secret};
        }
    }
}