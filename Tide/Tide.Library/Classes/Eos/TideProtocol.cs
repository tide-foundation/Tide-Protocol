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
using System.Linq;
using EosSharp.Core;
using EosSharp.Core.Api.v1;
using EosSharp.Core.Providers;
using Tide.Library.Classes.Encryption;
using Tide.Library.Models;
using Tide.Library.Models.Interfaces;
using Action = EosSharp.Core.Api.v1.Action;



namespace Tide.Library.Classes.Eos {
    public class TideProtocol : ITideProtocol {
        private const string AccountCharacters = "abcdefghijklmnopqrstuvwxyz1234";
        private readonly EosSharp.Eos _client;

        private readonly Random _random = new Random();
        private readonly Settings _settings;

        public TideProtocol(Settings settings) {
            _settings = settings;

            _client = new EosSharp.Eos(new EosConfigurator {
                HttpEndpoint = settings.Blockchain.BlockchainEndpoint,
                ChainId = settings.Blockchain.BlockchainChainId,
                ExpireSeconds = 60,
                SignProvider = new DefaultSignProvider(settings.Blockchain.Keys)
            });
        }

        #region Misc

        public bool AccountExists(string username) {
            return _client.GetTableRows<User>(new GetTableRowsRequest {
                code = _settings.Blockchain.AuthenticationContract,
                scope = _settings.Blockchain.AuthenticationContract,
                table = "users",
                lower_bound = username.ConvertToUint64().ToString(),
                upper_bound = (username.ConvertToUint64() + 1).ToString(),
                limit = 1
            }).Result.rows.Any();
        }

        public ulong HashUsername(string data) {
            return Cryptide.Instance.HashUsername(data).username.ConvertToUint64();
        }

        #endregion

        #region Master

        public TideResponse CreateVendor(string blockchainAccount, string username, string publicKey, string description) {
            var data = new {
                payer = _settings.Instance.Account,
                account = blockchainAccount,
                username = username.ConvertToUint64(),
                public_key = publicKey,
                desc = description
            };

            return Push(_settings.Blockchain.VendorContract, EosHelpers.CreateVendor, _settings.Instance.Account, data);
        }

        #endregion

        #region Vendor

   
        public TideResponse InitializeAccount(string publicKey, string username) {
            var accountResult = CreateBlockchainAccount(publicKey);
            if (!accountResult.Success) return accountResult;
            
            var data = new {
                vendor_username = HashUsername(_settings.Instance.Username),
                account = accountResult.Content.ToString(),
                account_username = username.ConvertToUint64(),
                time = EosHelpers.GetEpoch()
            };

            var result = Push(_settings.Blockchain.AuthenticationContract, EosHelpers.InitializeAccount, _settings.Instance.Account, data);
            return new TideResponse(result.Success, result.Success ? accountResult.Content.ToString() : null, result.Success ? null : result.Error);
        }

    
        public TideResponse ConfirmAccount(string username) {
            var data = new {
                vendor_username = HashUsername(_settings.Instance.Username),
                account_username = username.ConvertToUint64()
            };

            return Push(_settings.Blockchain.AuthenticationContract, EosHelpers.ConfirmAccount, _settings.Instance.Account, data);
        }

        #endregion

        #region Ork

     
        public TideResponse GetNodes(string username) {
            throw new NotImplementedException();
        }

        public TideResponse PostFragment(AuthenticationModel model) {
            throw new NotImplementedException();
        }

        public TideResponse GetFragment(AuthenticationModel model) {
            throw new NotImplementedException();
        }

        #endregion

        #region Helpers

        private TideResponse Push(string contract, string action, string auth, object data) {
            return Push(contract, action, new List<string> {auth}, data);
        }

        private TideResponse Push(string contract, string action, IEnumerable<string> auth, object data) {
            try {
                var content = _client.CreateTransaction(new Transaction {
                    actions = new List<Action> {
                        new Action {
                            account = contract,
                            name = action,
                            authorization = auth.Select(a => new PermissionLevel {actor = a, permission = "active"}).ToList(),
                            data = data
                        }
                    }
                }).Result;

                return new TideResponse(true, content);
            }
            catch (Exception e) {
                return new TideResponse(e.Message);
            }
        }

        private TideResponse CreateBlockchainAccount(string publicKey) {
            var account = $"tide{new string(Enumerable.Repeat(AccountCharacters, 8).Select(s => s[_random.Next(s.Length)]).ToArray())}" ;
            var data = new {
                creator = "xtidemasterx",
                name = account,
                owner = new Authority {
                    threshold = 1,
                    keys = new List<AuthorityKey> {
                        new AuthorityKey {
                            key = publicKey,
                            weight = 1
                        }
                    },
                    accounts = new List<AuthorityAccount>(),
                    waits = new List<AuthorityWait>()
                },
                active = new Authority {
                    threshold = 1,
                    keys = new List<AuthorityKey> {
                        new AuthorityKey {
                            key = publicKey,
                            weight = 1
                        }
                    },
                    accounts = new List<AuthorityAccount>(),
                    waits = new List<AuthorityWait>()
                }
            };

            var result = Push("eosio", "newaccount", "xtidemasterx", data);
            return new TideResponse(result.Success, result.Success ? account : null);
        }

        #endregion
    }
}