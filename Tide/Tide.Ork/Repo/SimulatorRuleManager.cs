using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public class SimulatorRuleManager : SimulatorManagerBase<RuleVault>, IRuleManager {

        public SimulatorRuleManager(string orkId, SimulatorClient client, AesKey key) : base(orkId, client, key)
        {
        }
        
        public Task ConfirmAll(Guid ownerId) => throw new NotImplementedException("Do not invoke confirm in simulator manager");

        //TODO: Ask Matt for help
        public Task<List<RuleVault>> GetSetBy(Guid ownerId)
        {
            throw new NotImplementedException();
        }

        //TODO: Ask Matt for help
        public Task<List<RuleVault>> GetSetBy(Guid ownerId, ulong tag, Guid keyId)
        {
            throw new NotImplementedException();
        }
    }
}