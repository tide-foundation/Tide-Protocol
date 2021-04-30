using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.VendorSdk.Classes;

namespace Tide.Vendor.Classes
{
    public class MemoryVendorRepo : IVendorRepo
    {
        public List<string> _orkUrls { get; }

        private readonly ConcurrentDictionary<Guid, Account> _items;

        public MemoryVendorRepo(Settings settings)
        {
            _orkUrls = settings.OrkUrls;
            _items = new ConcurrentDictionary<Guid, Account>();
        }

        public Task CreateUser(Guid vuid, AesKey auth, List<string> orks)
        {
            _items[vuid] = new Account
            {
                Vuid = vuid,
                Key = auth,
                Orks = orks,
                State = TransactionState.New
            };

            return Task.CompletedTask;
        }

        public async Task<AesKey> GetKey(Guid vuid)
        {
            return (await GetAccount(vuid))?.Key;
        }

        public Task ConfirmUser(Guid vuid)
        {
            return ChangeState(vuid, TransactionState.Confirmed);
        }

        public Task RollbackUser(Guid vuid)
        {
            return ChangeState(vuid, TransactionState.Reverted);
        }

        public Task<List<string>> GetListOrks() => Task.FromResult(_orkUrls);

        private async Task ChangeState(Guid vuid, TransactionState state)
        {
            var account = (await GetAccount(vuid));
            if (account == null)
                return;

            account.State = state;
            _items[vuid] = account;
        }

        private Task<Account> GetAccount(Guid vuid)
        {
            if (!_items.ContainsKey(vuid))
                return Task.FromResult<Account>(null);

            return Task.FromResult(_items[vuid]);
        }

        public async Task<List<string>> GetListOrks(Guid vuid)
        {
            return (await GetAccount(vuid))?.Orks;
        }

        protected class Account
        {
            public Guid Vuid { get; set; }
            public AesKey Key { get; set; }
            public List<string> Orks { get; set; }
            public TransactionState State { get; set; }
        }
    }
}