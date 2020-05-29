// Tide Protocol - Infrastructure for the Personal Data economy
// Copyright (C) 2019 Tide Foundation Ltd
// 
// This program is free software and is subject to the terms of 
// the Tide Community Open Source License as published by the 
// Tide Foundation Limited. You may modify it and redistribute 
// it in accordance with and subject to the terms of that License.
// This program is distributed WITHOUT WARRANTY of any kind, 
// including without any implied warranty of MERCHANTABILITY or 
// FITNESS FOR A PARTICULAR PURPOSE.
// See the Tide Community Open Source License for more details.
// You should have received a copy of the Tide Community Open 
// Source License along with this program.
// If not, see https://tide.org/licenses_tcosl-1-0-en

using System;
using System.Collections.Generic;
using System.Numerics;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Classes
{
    public interface IKeyManager
    {
        bool Exist(Guid user);
        BigInteger GetAuthShare(Guid user);
        AesKey GetSecret(Guid user);
        KeyVault GetByUser(Guid user);
        void SetOrUpdateKey(Guid user, BigInteger authShare, BigInteger keyShare, AesKey secret);
    }

    public class MemoryKeyManager : IKeyManager
    {
        private readonly Dictionary<Guid, KeyVault> _items;

        public MemoryKeyManager()
        {
            _items = new Dictionary<Guid, KeyVault>();
        }

        public bool Exist(Guid user)
        {
            return _items.ContainsKey(user);
        }

        public BigInteger GetAuthShare(Guid user)
        {
            return Exist(user) ? _items[user].AuthShare : BigInteger.Zero;
        }

        public AesKey GetSecret(Guid user)
        {
            return Exist(user) ? _items[user].Secret : null;
        }

        public KeyVault GetByUser(Guid user)
        {
            return Exist(user) ? _items[user] : null;
        }

        public void SetOrUpdateKey(Guid user, BigInteger authShare, BigInteger keyShare, AesKey secret)
        {
            _items[user] = new KeyVault { User = user, AuthShare = authShare, KeyShare = keyShare, Secret = secret };
        }
    }

    public class KeyVault
    {
        public Guid User { get; set; }
        public BigInteger AuthShare { get; set; }
        public BigInteger KeyShare { get; set; }
        public AesKey Secret { get; set; }
    }
}
