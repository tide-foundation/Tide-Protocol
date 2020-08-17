using System;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public class SimulatorCvkManager : SimulatorManagerBase<CvkVault>, ICvkManager {

        public SimulatorCvkManager(string orkId, SimulatorClient client, AesKey key) : base(orkId, client, key)
        {
        }

        public Task Confirm(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task Rollback(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}