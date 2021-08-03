using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public class SimulatorRuleManager : SimulatorManagerCipherBase<RuleVault>, IRuleManager {
        protected override string TableName => "rules";
        protected override string Contract => "Permissions";
        public static Guid MaxID => Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        public SimulatorRuleManager(string orkId, SimulatorClient client, AesKey key) : base(orkId, client, key)
        {
        }
        
        public Task ConfirmAll(Guid ownerId) => Task.CompletedTask; //throw new NotImplementedException("Do not invoke confirm in simulator manager");

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

        public async Task<List<RuleVault>> GetSetBy(Guid ownerId, ICollection<ulong> tags, Guid keyId)
        {
            return (await GetAll()).Where(rule => rule.OwnerId == ownerId
                && (tags.Contains(rule.Tag) || rule.Tag == ulong.MaxValue)
                && (rule.KeyId == keyId || rule.KeyId == MaxID)).ToList();
        }
    }
}