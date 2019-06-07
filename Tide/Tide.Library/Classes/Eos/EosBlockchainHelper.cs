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
using Tide.Library.Models;
using Tide.Library.Models.Interfaces;
using Action = EosSharp.Core.Api.v1.Action;

namespace Tide.Library.Classes.Eos {
    public class EosBlockchainHelper : IBlockchainHelper {
        private const string AccountCharacters = "abcdefghijklmnopqrstuvwxyz1234";
        private readonly EosSharp.Eos _client;

        private readonly Random _random = new Random();
        private readonly Settings _settings;

        public EosBlockchainHelper(Settings settings) {
            _settings = settings;

            _client = new EosSharp.Eos(new EosConfigurator {
                HttpEndpoint = settings.Blockchain.BlockchainEndpoint,
                ChainId = settings.Blockchain.BlockchainChainId,
                ExpireSeconds = 60,
                SignProvider = new DefaultSignProvider(settings.Keys)
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

        #endregion

        #region Master

        /// <summary>
        ///     Creates a blockchain account on the implemented blockchain
        /// </summary>
        /// <param name="publicKey">Public key to be used for the blockchain account</param>
        /// <returns>Content: Blockchain transaction ID</returns>
        public TideResponse CreateBlockchainAccount(string publicKey) {
            var data = new {
                creator = "eosio",
                name = new string(Enumerable.Repeat(AccountCharacters, 12).Select(s => s[_random.Next(s.Length)]).ToArray()),
                OwnerKey = publicKey,
                ActiveKey = publicKey
            };

            return Push("create", "account", "eosio", data);
        }

        /// <summary>
        ///     Creates a top-level vendor account in which a business can run and have user-accounts signed up below it.
        /// </summary>
        /// <param name="model">
        ///     Payer: The account paying for the account.
        ///     Account: Blockchain account for the new vendor
        ///     Username: Tide Username for the new vendor
        ///     PublicKey: Elgamal public key the vendor will use for encryption
        ///     Description: A small synopsys of the vendor
        /// </param>
        /// <returns>Content: Blockchain transaction ID</returns>
        public TideResponse CreateVendor(CreateVendorModel model) {
            var data = new {
                payer = _settings.Instance.Account,
                account = model.Account,
                username = model.Username.ConvertToUint64(),
                public_key = model.PublicKey,
                desc = model.Description
            };

            return Push(_settings.Blockchain.AuthenticationContract, EosHelpers.CreateVendor, _settings.Instance.Account, data);
        }

        #endregion

        #region Vendor

        /// <summary>
        ///     Initializes an account to be created. This is step one and can not be skipped.
        /// </summary>
        /// <param name="username">The username for the new Tide account</param>
        /// <returns>Content: Blockchain transaction ID</returns>
        public TideResponse InitializeAccount(string username) {
            var data = new {
                vendor = _settings.Instance.Account,
                username = username.ConvertToUint64(),
                time = EosHelpers.GetEpoch()
            };

            return Push(_settings.Blockchain.AuthenticationContract, EosHelpers.InitializeAccount, _settings.Instance.Account, data);
        }

        /// <summary>
        ///     Finalizes the account once the client has confirmed all fragments have successfully been stored by the Orks.
        ///     Alternatives if the user decided not to use Orks and has taken note of the keys.
        /// </summary>
        /// <param name="username">The Tide username to confirm</param>
        /// <returns>Content: Blockchain transaction ID</returns>
        public TideResponse ConfirmAccount(string username) {
            var data = new {
                vendor = _settings.Instance.Account,
                username = username.ConvertToUint64()
            };

            return Push(_settings.Blockchain.AuthenticationContract, EosHelpers.ConfirmAccount, _settings.Instance.Account, data);
        }

        #endregion

        #region Ork

        /// <summary>
        ///     Gathers the ork nodes which the Tide user used to distribute their key
        /// </summary>
        /// <param name="username">Username of the Tide account</param>
        /// <returns>Content: Array of ork nodes</returns>
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

        #endregion
    }
}