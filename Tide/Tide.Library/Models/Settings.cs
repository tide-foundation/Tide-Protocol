using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Library.Models
{
    public class Settings
    {
        public Blockchain Blockchain { get; set; }
        public Instance Instance { get; set; }
    }

    public class Blockchain {
        public string BlockchainChainId { get; set; }
        public string BlockchainEndpoint { get; set; }
        public string AuthenticationContract { get; set; }
        public string UsersTable { get; set; }
        public string FragmentsTable { get; set; }
    }

    public class Instance {
        public string Account { get; set; }
        public string EncryptionPublicKey { get; set; }
        public string EncryptionPrivateKey { get; set; }
        public string ChainPrivateKey { get; set; }
        public string AesPassword { get; set; }
        public string TokenKey { get; set; }
    }
}