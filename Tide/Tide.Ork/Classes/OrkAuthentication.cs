// 
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
//

using Microsoft.Extensions.Logging;
using Tide.Library.Classes.Eos;
using Tide.Library.Models;
using Tide.Library.Models.Interfaces;

namespace Tide.Ork.Classes {
    public class OrkAuthentication : IOrkAuthentication {
        private readonly ITideProtocol _helper;
        private readonly ILogger _logger;

        public OrkAuthentication(Settings settings, ILoggerFactory logger) {
            _logger = logger.CreateLogger($"Instance-{settings.Instance.Account}");
            _helper = new Library.Classes.Eos.TideProtocol(settings);
        }

        public TideResponse GetNodes(AuthenticationModel model) {
            return _helper.GetNodes(model.Username);
        }

        public TideResponse PostFragment(AuthenticationModel model) {
            return _helper.PostFragment(model);
        }

        public TideResponse GetFragment(AuthenticationModel model) {
            return _helper.GetFragment(model);
        }
    }
}