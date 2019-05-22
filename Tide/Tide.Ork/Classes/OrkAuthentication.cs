using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EOS.Client;
using EOS.Client.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Tide.Library.Classes;
using Tide.Library.Models;
using Tide.Library.Models.Interfaces;

namespace Tide.Ork.Classes
{
    public class OrkAuthentication : IOrkAuthentication
    {
        private readonly ILogger _logger;
        private readonly Settings _settings;
        private readonly IBlockchainHelper _helper;

        public OrkAuthentication(Settings settings, ILoggerFactory logger, IBlockchainHelper helper) {
            _settings = settings;
            _logger = logger.CreateLogger($"Instance-{settings.Instance.Account}");
            _helper = helper;
        }

        public TideResponse GetNodes(AuthenticationModel model) {
            return _helper.GetNodes(model.Username);
        }

        public TideResponse PostFragment(AuthenticationModel model)
        {
            return _helper.PostFragment(model);
        }

        public TideResponse GetFragment(AuthenticationModel model) {
            return _helper.GetFragment(model);
        }
    }
}
    