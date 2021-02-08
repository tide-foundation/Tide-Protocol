using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tide.Core;

namespace Tide.Ork.Repo
{
    public class CacheRuleManager : CacheManager<RuleVault>, IRuleManager
    {
        private readonly IRuleManager _manager;

        public static Guid MaxID => MemoryRuleManager.MaxID;

        public CacheRuleManager(IMemoryCache cache, IRuleManager manager) : base(cache, manager)
        {
            _manager = manager;
        }

        public async Task<List<RuleVault>> GetSetBy(Guid ownerId, ulong tag, Guid keyId)
        {
            var fromRemote =   await _manager.GetSetBy(ownerId, tag, keyId);
            var fromLocal = GetEnumerable().Where(rule => rule.OwnerId == ownerId
                && (rule.Tag == tag || rule.Tag == ulong.MaxValue)
                && (rule.KeyId == keyId || rule.KeyId == MaxID));

            return fromLocal.Concat(fromRemote).ToList();
        }

        public async Task<List<RuleVault>> GetSetBy(Guid ownerId)
        {
            var fromRemote = await _manager.GetSetBy(ownerId);
            var fromLocal = GetEnumerable().Where(rule => rule.OwnerId == ownerId).ToList();

            return fromLocal.Concat(fromRemote).ToList();
        }

        public virtual Task ConfirmAll(Guid ownerId)
        {
            return Task.WhenAll(GetEnumerable().Where(rule => rule.OwnerId == ownerId)
                .Select(rule => Confirm(rule.Id)));
        }

        public async Task<List<RuleVault>> GetSetBy(Guid ownerId, ICollection<ulong> tags, Guid keyId)
        {
            var fromRemote =   await _manager.GetSetBy(ownerId, tags, keyId);
            var fromLocal = GetEnumerable().Where(rule => rule.OwnerId == ownerId
                && (tags.Contains(rule.Tag) || rule.Tag == ulong.MaxValue)
                && (rule.KeyId == keyId || rule.KeyId == MaxID));

            return fromLocal.Concat(fromRemote).ToList();
        }
    }
}