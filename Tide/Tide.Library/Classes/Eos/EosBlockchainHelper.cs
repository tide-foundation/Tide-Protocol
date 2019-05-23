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

using System;
using System.Collections.Generic;
using System.Text;
using EOS.Client;
using EOS.Client.Models;
using Tide.Library.Models;
using Tide.Library.Models.Interfaces;

namespace Tide.Library.Classes.Eos
{
    public class EosBlockchainHelper : IBlockchainHelper
    {
        private readonly EosClient _client;
        private readonly Settings _settings;

        public EosBlockchainHelper(Settings settings) {
            _settings = settings;
            _client = new EosClient(new Uri(settings.Blockchain.BlockchainEndpoint), new EosWallet(new List<string> { settings.Instance.ChainPrivateKey }));
        }

        #region Vendor

        public TideResponse InitializeAccount(string username) {
            var data = new Dictionary<string, object> {
                { "vendor", _settings.Instance.Account },
                { "username", username.ConvertToUint64() },
                { "time", EosHelpers.GetEpoch() }
            };
            return Push(_settings.Blockchain.AuthenticationContract, EosHelpers.InitializeAccount, _settings.Instance.Account, data);
        }

        public TideResponse ConfirmAccount(string username)
        {
            var data = new Dictionary<string, object> {
                { "vendor", _settings.Instance.Account },
                { "username", username.ConvertToUint64() }
            };
            return Push(_settings.Blockchain.AuthenticationContract, EosHelpers.ConfirmAccount, _settings.Instance.Account, data);
        }

        #endregion

        #region Ork

        public TideResponse GetNodes(string username)
        {
            throw new NotImplementedException();
        }

        public TideResponse PostFragment(AuthenticationModel model)
        {
            throw new NotImplementedException();
        }

        public TideResponse GetFragment(AuthenticationModel model)
        {
            throw new NotImplementedException();
        }

        #endregion


        private TideResponse Push(string contract, string action, string auth, IDictionary<string, object> data)
        {
            try {
                var content = _client.PushActionsAsync(new[]
                {
                    new EOS.Client.Models.Action()
                    {
                        Account = contract,
                        Name = action,
                        Authorization = new []
                        {
                            new Authorization
                            {
                                Actor = auth,
                                Permission = "active"
                            }
                        },
                        Data = data
                    }
                }).Result;
                return new TideResponse(true,content);
            }
            catch (Exception e) {
                return new TideResponse(e.Message);
            }
        }
    }
}
