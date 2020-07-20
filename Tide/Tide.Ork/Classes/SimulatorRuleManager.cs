using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Classes {
    public class SimulatorRuleManager : SimulatorManagerBase<RuleVault>, IRuleManager {

        public SimulatorRuleManager(string orkId, SimulatorClient client, AesKey key) : base(orkId, client, key)
        {
        }

        public Task<List<RuleVault>> GetSetByOwner(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}