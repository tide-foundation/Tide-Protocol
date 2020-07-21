using System;
using System.Numerics;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Classes {
    public class SimulatorKeyManager : SimulatorManagerBase<KeyVault>, IKeyManager {

        public SimulatorKeyManager(string orkId, SimulatorClient client, AesKey key) : base(orkId, client, key)
        {
        }

        public async Task<BigInteger> GetAuthShare(Guid userId) {
            var user = await GetById(userId);
            return user != null ? user.AuthShare : BigInteger.Zero;
        }
    }
}