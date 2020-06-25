using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Classes
{
    public class MemoryKeyManager : IKeyManager
    {
        private readonly Dictionary<string, string> _items;

        public MemoryKeyManager()
        {
            _items = new Dictionary<string, string>();
        }

        public Task<bool> Exist(Guid user)
        {
            return Task.FromResult(_items.ContainsKey(user.ToString()));
        }

        public Task<BigInteger> GetAuthShare(Guid user)
        {
            var usr = user.ToString();
            if (!_items.ContainsKey(usr))
                return Task.FromResult(BigInteger.Zero);
             
            return Task.FromResult(KeyVault.Parse(_items[usr]).AuthShare);
        }

        public Task<AesKey> GetSecret(Guid user)
        {
            var usr = user.ToString();
            if (!_items.ContainsKey(usr))
                return Task.FromResult<AesKey>(null);

            return Task.FromResult(KeyVault.Parse(_items[usr]).Secret);
        }

        public Task<KeyVault> GetByUser(Guid user)
        {
            var usr = user.ToString();
            if (!_items.ContainsKey(usr))
                return Task.FromResult<KeyVault>(null);

            return Task.FromResult(KeyVault.Parse(_items[usr]));
        }

        public Task<TideResponse> SetOrUpdate(KeyVault account)
        {
            _items[account.User.ToString()] = account.ToString();
            return Task.FromResult(new TideResponse());
        }
    }
}