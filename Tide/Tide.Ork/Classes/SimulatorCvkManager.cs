using System;
using Tide.Core;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Classes {
    public class SimulatorCvkManager : SimulatorManagerBase<CvkVault>, ICvkManager {

        public SimulatorCvkManager(string orkId, SimulatorClient client, AesKey key) : base(orkId, client, key)
        {
        }
    }
}