using System;
using Tide.Core;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public class SimulatorDnsManager : SimulatorManagerPublicBase<DnsEntry, DnsEntry>, IDnsManager {
        protected override string TableName => "Dns";
        protected override string Contract => "Accounts";
        public SimulatorDnsManager(string orkId, SimulatorClient client) : base(orkId, client)
        {
        }
    }
}