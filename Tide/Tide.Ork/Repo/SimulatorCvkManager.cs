using System;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public class SimulatorCvkManager : SimulatorManagerBase<CvkVault>, ICvkManager {

        public SimulatorCvkManager(string orkId, SimulatorClient client, AesKey key) : base(orkId, client, key)
        {
        }
    }
}