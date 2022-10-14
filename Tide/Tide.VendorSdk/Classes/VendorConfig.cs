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

using Microsoft.IdentityModel.Tokens;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ed;

namespace Tide.VendorSdk.Classes
{
    public class VendorConfig
    {
        public Ed25519Key PrivateKey { get; set; }
        public AesKey SecretKey { get; set; }
        
        public SecurityKey GetSessionKey() => new SymmetricSecurityKey(SecretKey.Hash(new byte[32]));
    }
}