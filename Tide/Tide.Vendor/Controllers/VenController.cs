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
using Microsoft.AspNetCore.Mvc;
using Tide.VendorSdk.Controllers;
using Tide.VendorSdk.Classes;
using Microsoft.Extensions.Logging;

namespace Tide.Ork.Controllers {
    [ApiController]
    [Route("Vendor")]
    public class VenController : VendorController {
        public VenController(IOrkRepo repo, VendorConfig config, ILogger<VendorController> logger) : base(repo, config, logger)
        {

        }
    }
}