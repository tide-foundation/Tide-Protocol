using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Tide.Simulator.Classes
{
    public class SimulatorHub : Hub {
        private BlockchainContext _context;

        public SimulatorHub(BlockchainContext context) {
            _context = context;
        }

        public async Task Populate()
        {
            await Clients.All.SendAsync("Populate", _context.Data.ToList());
        }
    }
}
