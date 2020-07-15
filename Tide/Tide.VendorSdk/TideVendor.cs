using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Tide.Core;
using Tide.VendorSdk.Classes;
using Tide.VendorSdk.Configuration;

namespace Tide.VendorSdk
{
    public class TideVendor
    {
        private static SimulatorClient _client;
        private readonly TideConfiguration _config;

        public TideVendor(TideConfiguration config) {
            _config = config;
            _client = new SimulatorClient(_config.VendorId);
        }

        #region Onboarding  

        /// <summary>
        /// Gathers user nodes
        /// </summary>
        /// <param name="username">The username to gather nodes</param>
        /// <returns></returns>
        public TideResponse GetUserNodes(string username)
        {
            var (success, content) = _client.GetUserNodes(username);
            return new TideResponse(success, success ? content : null, !success ? content : null);
        }

        public TideResponse CreateUser(string username, List<string> desiredOrks)
        {
            var (success, error) = _client.CreateUser(username, desiredOrks);
            return new TideResponse(success, null, error);
        }

        public TideResponse ConfirmUser(string username)
        {
            var (success, error) = _client.ConfirmUser(username);
            return new TideResponse(success, null, error);
        }

        public TideResponse RollbackUser(string username)
        {
            var (success, error) = _client.RollbackUser(username);
            return new TideResponse(success, null, error);
        }

        #endregion
    }
}
