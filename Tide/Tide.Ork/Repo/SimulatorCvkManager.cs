using System;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public class SimulatorCvkManager : SimulatorManagerCipherBase<CvkVault>, ICvkManager {
        protected override string TableName => "cvks";
        protected override string Contract => "Authentication";

        public SimulatorCvkManager(string orkId, SimulatorClient client, AesKey key) : base(orkId, client, key)
        {
        }

        public Task Confirm(Guid id) => Task.CompletedTask; //throw new NotImplementedException("Do not invoke confirm in simulator manager");
    }
}