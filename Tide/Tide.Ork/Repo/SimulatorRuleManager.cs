using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public class SimulatorRuleManager : SimulatorManagerBase<RuleVault>, IRuleManager {
        protected override string TableName => "rules";
        public static Guid MaxID => Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        public SimulatorRuleManager(string orkId, SimulatorClient client, AesKey key) : base(orkId, client, key)
        {
        }
        
        public Task ConfirmAll(Guid ownerId) => throw new NotImplementedException("Do not invoke confirm in simulator manager");

        public async Task<List<RuleVault>> GetSetBy(Guid ownerId)
        {
            return (await GetAll()).Where(rule => rule.OwnerId == ownerId).ToList();
        }

        public async Task<List<RuleVault>> GetSetBy(Guid ownerId, ulong tag, Guid keyId)
        {
            return (await GetAll()).Where(rule => rule.OwnerId == ownerId
                && (rule.Tag == tag || rule.Tag == ulong.MaxValue)
                && (rule.KeyId == keyId || rule.KeyId == MaxID)).ToList();
        }
    }
}