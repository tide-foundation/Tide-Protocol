using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Tide.Ork.Classes {
    public class EnclaveHub : Hub {
        public static readonly Dictionary<string, IClientProxy> ClientMapping = new Dictionary<string, IClientProxy>();

        public async Task RequestSession() {
            var id = Guid.NewGuid().ToString();
            ClientMapping[id] = Clients.Caller;
            await Clients.Caller.SendAsync("openSession", id);
        }
    }
}