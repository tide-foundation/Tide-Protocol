using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tide.Core;

namespace Tide.Ork.Repo
{
    public class MemoryRuleManager : MemoryManagerBase<RuleVault>, IRuleManager
    {
        public static Guid MaxID => Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        public Task ConfirmAll(Guid ownerId)
        {
            throw new NotImplementedException();
        }

        public Task<List<RuleVault>> GetSetBy(Guid ownerId, ulong tag, Guid keyId)
        {
            return Task.FromResult(GetEnumerable().Where(rule => rule.OwnerId == ownerId
                && (rule.Tag == tag || rule.Tag == ulong.MaxValue)
                && (rule.KeyId == keyId || rule.KeyId == MaxID)).ToList());
        }

        public Task<List<RuleVault>> GetSetBy(Guid ownerId)
        {
            return Task.FromResult(GetEnumerable().Where(rule => rule.OwnerId == ownerId).ToList());
        }
    }
}