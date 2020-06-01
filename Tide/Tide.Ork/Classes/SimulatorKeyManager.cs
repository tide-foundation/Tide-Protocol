﻿using System;
using System.Numerics;
using Library;
using Tide.Encryption.AesMAC;
using Tide.Ork.Models;

namespace Tide.Ork.Classes {
    public class SimulatorKeyManager : IKeyManager {
        private SimulatorClient _client;

        public SimulatorKeyManager(SimulatorClient client) {
            _client = client;
        }

        public bool Exist(Guid user) {
            throw new NotImplementedException();
        }

        public BigInteger GetAuthShare(Guid user) {
            throw new NotImplementedException();
        }

        public AesKey GetSecret(Guid user) {
            throw new NotImplementedException();
        }

        public KeyVault GetByUser(Guid user) {
            throw new NotImplementedException();
        }

        public void SetOrUpdateKey(Guid user, BigInteger authShare, BigInteger keyShare, AesKey secret) {
            throw new NotImplementedException();
        }
    }
}