using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tide.Core;

namespace Tide.Ork.Classes
{
    public class MemoryRuleManager : MemoryManagerBase<RuleVault>, IRuleManager
    {
        public Task<List<RuleVault>> GetSetByOwner(Guid id)
        {
            return Task.FromResult(GetEnumerable().Where(rule => rule.OwnerId == id).ToList());
        }
    }
}