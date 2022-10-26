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
using System.Linq;
using System.Numerics;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ed;
using static Tide.Ork.Models.CVKRandomResponse;

namespace Tide.Ork.Controllers
{
    public class CVKRandRegistrationReq
    {
        public CVKRandomShareResponse[] Shares { get; set; }
        public Ed25519Key CvkPub;
        public AesKey CvkiAuth { get; set; }
        public Guid KeyId;
        public byte[] Signature;
        public string entry { get; set;}
        public DnsEntry GetEntry() => DnsEntry.Parse(entry);
        public string Cvki {get; set;}
        public BigInteger GetCvki() => BigInteger.Parse(Cvki);

        public string Cvk2i {get; set;}
        public BigInteger GetCvk2i() => BigInteger.Parse(Cvk2i);

        internal BigInteger ComputeCvk() => Shares.Select(shr => shr.CvkVal)
            .Aggregate((sum, cvk) => (sum + cvk) % Ed25519.N);

        internal BigInteger ComputeCvk2() => Shares.Select(shr => shr.Cvk2Val)
            .Aggregate((sum, cvk2) => (sum + cvk2) % Ed25519.N);


    }
}