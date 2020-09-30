using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Tide.Simulator.Classes
{
    public class SimulatorHub : Hub {
        private readonly IBlockLayer _blockchain;

        public SimulatorHub(IBlockLayer blockchain) {
            _blockchain = blockchain;
        }

        public async Task Populate()
        {
            await Clients.All.SendAsync("Populate", _blockchain.ReadHistoric());
        }
    }
}
