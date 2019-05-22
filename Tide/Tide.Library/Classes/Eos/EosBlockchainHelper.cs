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
