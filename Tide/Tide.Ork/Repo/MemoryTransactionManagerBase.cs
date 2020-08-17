using System;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption;

namespace Tide.Ork.Repo
{
    public class MemoryTransactionManagerBase<T> : MemoryManagerBase<T> where T : SerializableByteBase<T>, IGuid, ITransactionState, new()
    {
        public Task Confirm(Guid id) => ChangeState(id, TransactionState.Confirmed);

        public Task Rollback(Guid id) => ChangeState(id, TransactionState.Reverted);

        private async Task ChangeState(Guid id, TransactionState state)
        {
            var account = (await GetById(id));
            if (account == null)
                return;

            account.State = state;
            await SetOrUpdate(account);
        }
    }
}