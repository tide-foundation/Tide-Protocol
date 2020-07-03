using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Classes
{
    public class MemoryCvkManager : ICvkManager
    {
        private readonly Dictionary<string, string> _items;

        public MemoryCvkManager()
        {
            _items = new Dictionary<string, string>();
        }

        public Task<bool> Exist(Guid user)
        {
            return Task.FromResult(_items.ContainsKey(user.ToString()));
        }

        public Task<BigInteger> GetShare(Guid user)
        {
            var usr = user.ToString();
            if (!_items.ContainsKey(usr))
                return Task.FromResult(BigInteger.Zero);
             
            return Task.FromResult(CvkVault.Parse(_items[usr]).CVKi);
        }

        public Task<AesKey> GetAuth(Guid user)
        {
            var usr = user.ToString();
            if (!_items.ContainsKey(usr))
                return Task.FromResult<AesKey>(null);

            return Task.FromResult(CvkVault.Parse(_items[usr]).CvkAuth);
        }

        public Task<CvkVault> GetByUser(Guid user)
        {
            var usr = user.ToString();
            if (!_items.ContainsKey(usr))
                return Task.FromResult<CvkVault>(null);

            return Task.FromResult(CvkVault.Parse(_items[usr]));
        }

        public Task<TideResponse> SetOrUpdate(CvkVault account)
        {
            _items[account.User.ToString()] = account.ToString();
            return Task.FromResult(new TideResponse());
        }
    }
}