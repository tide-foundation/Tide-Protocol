using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption.AesMAC;

namespace Tide.VendorSdk.Classes
{
    public class VendorRepo : IVendorRepo
    {
        private readonly ConcurrentDictionary<Guid, Account> _items;

        public VendorRepo()
        {
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

        private async Task<string[]> GetListById(IEnumerable<Guid> selected)
        {
            var orks = (await GetListOrks()).Select(ork => new
            {
                url = ork,
                id = IdGenerator.Seed(new Uri(ork)).Guid
            }).ToList();

            return orks.Where(ork => selected.Contains(ork.id)).Select(ork => ork.url).ToArray();
        }


        public Task<List<string>> GetListOrks()
        {
            return Task.FromResult(new List<string> {
                "https://dork1.azurewebsites.net/discover",
                "https://dork2.azurewebsites.net/discover",
                "https://dork3.azurewebsites.net/discover"
            });
        }

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