using System;

namespace Tide.Core
{
    public interface ITransactionState
    {
        TransactionState State { get; set; }
    }
}
