using System;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public class SimulatorAuditManager
    {
        protected readonly SimulatorClient _client;
        protected readonly string _orkId;
        const string table = "Log";
        const string index = "AuditTrail";
        const string contract = "AuditTrail";

        public SimulatorAuditManager(string orkId, SimulatorClient client) {
            _orkId = orkId;
            _client = client;
        }

        public async Task<TideResponse> Report(string logs)
        {
            var result = await _client.Post(contract, table, _orkId, index, logs);
             if(result.success) return new TideResponse(true);
            return new TideResponse(false,null,result.error);
        }
    }
}
