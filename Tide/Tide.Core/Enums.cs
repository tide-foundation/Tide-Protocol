namespace Tide.Core {
    public enum Contract {
        Unset = 0,
        Authentication = 1
    }

    public enum Table {
        Unset = 0,
        Vault = 1,
        Users = 2,
        Orks = 3
    }

    public enum TransactionState { 
        New = 0,
        Confirmed = 1,
        Reverted = 2
    };
}