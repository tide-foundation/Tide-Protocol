using System;
using System.Collections.Generic;
using System.Text;
using Tide.Library.Models;
using Tide.Library.Models.Interfaces;

namespace Tide.Sdk.Classes.Eos
{
    public class VendorAuthentication : IVendorAuthentication
    {
        private readonly IBlockchainHelper _helper;
        private readonly Settings _settings;

        public VendorAuthentication(Settings settings, IBlockchainHelper helper)
        {
            _settings = settings;
            _helper = helper;
        }

        public TideResponse InitializeAccount(AuthenticationModel model) {
            return _helper.InitializeAccount(model.Username);
        }

        public TideResponse ConfirmAccount(AuthenticationModel model) {
            return _helper.ConfirmAccount(model.Username);
        }
    }
}
