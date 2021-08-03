using System;
using Tide.Core;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public class SimulatorDnsManager : SimulatorManagerPublicBase<DnsEntry, DnsEntry>, IDnsManager {
        protected override string TableName => "Dns";
        protected override string Contract => "Authentication";
        public SimulatorDnsManager(SimulatorClient client) : base("Dns", client)
        {
        }
    }
}