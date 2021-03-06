﻿using System;
using System.Numerics;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public class SimulatorCmkManager : SimulatorManagerCipherBase<CmkVault>, ICmkManager {
        protected override string TableName => "cmks";
        protected override string Contract => "Authentication";

        public SimulatorCmkManager(string orkId, SimulatorClient client, AesKey key) : base(orkId, client, key)
        {
        }

        public async Task<BigInteger> GetPrism(Guid userId) {
            var user = await GetById(userId);
            return user != null ? user.Prismi : BigInteger.Zero;
        }

        public Task Confirm(Guid id) => Task.CompletedTask; //throw new NotImplementedException("Do not invoke confirm in simulator manager");
    }
}