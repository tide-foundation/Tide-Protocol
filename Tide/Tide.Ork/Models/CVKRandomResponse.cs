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
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Tide.Encryption.Ed;
using Tide.Encryption.SecretSharing;

namespace Tide.Ork.Models
{
    public class CVKRandomResponse
    {
        public Ed25519Point CvkPub { get; set; }
        public CVKRandomShareResponse[] Shares { get; set; }

        public CVKRandomResponse() {}

        public CVKRandomResponse(Ed25519Point cvkPub, IReadOnlyList<Point> cvks)
        {
            Debug.Assert(cvks != null && cvks.Any(), $"{nameof(cvks)} cannot be empty");

            CvkPub =cvkPub;
            Shares = cvks.Select((_, i) => new CVKRandomShareResponse
            {
                Id = new Guid(cvks[i].X.ToByteArray(true, true)),
                Cvk = cvks[i].Y.ToByteArray(true, true)
            }).ToArray();
        }

        public class CVKRandomShareResponse
        {
            public Guid Id { get; set; }
            public byte[] Cvk { get; set; }

            internal BigInteger CvkVal => new BigInteger(Cvk, true, true);
        }
    }
}
