using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Core
{
    public class User
    {
        /// <summary>
        /// The creator of the account
        /// </summary>
        public string Vendor { get; set; }

        /// <summary>
        /// The keys of the user
        /// </summary>
        public KeyVault Vault { get; set; }

        /// <summary>
        /// If this is an MSA, this will be a list of the child CVKs. Otherwise none
        /// </summary>
        public List<User> Users { get; set; }
    }
}
