using System;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public class SimulatorKeyIdManager : SimulatorManagerCipherBase<KeyIdVault>, IKeyIdManager {
        protected override string TableName => "keys";
        protected override string Contract => "Accounts";
        public SimulatorKeyIdManager(string orkId, SimulatorClient client, AesKey key) : base(orkId, client, key)
        {
        }

    }
}