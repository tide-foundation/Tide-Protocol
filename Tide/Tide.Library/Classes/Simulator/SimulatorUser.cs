using System;
using System.Collections.Generic;
using System.Text;
using Tide.Library.Classes.Eos;

namespace Tide.Library.Classes.Simulator
{
    public class SimulatorUser {
        public string Username { get; set; }
        public string Vendor { get; set; }
        public List<OrkLink> OrkLinks { get; set; }
    }
}
