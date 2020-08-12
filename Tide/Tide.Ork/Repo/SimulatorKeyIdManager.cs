using System;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public class SimulatorKeyIdManager : SimulatorManagerBase<KeyIdVault>, IKeyIdManager {

        public SimulatorKeyIdManager(string orkId, SimulatorClient client, AesKey key) : base(orkId, client, key)
        {
        }
    }
}