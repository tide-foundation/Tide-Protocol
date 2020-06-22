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
        /// Ork nodes used for this set of fragments
        /// </summary>
        public List<OrkStatus> Nodes { get; set; } = new List<OrkStatus>();

        /// <summary>
        /// If this is an MSA, this will be a list of the child CVKs. Otherwise none
        /// </summary>
        public List<User> Users { get; set; } = new List<User>();

        /// <summary>
        /// User remains pending until all fragments have been placed. The vendor should roll back after a set time if incomplete
        /// </summary>
        public UserStatus Status { get; set; } = UserStatus.Pending;
    }

    public class OrkStatus
    {
        public string Ork { get; set; }
        public bool Confirmed { get; set; }
    }

    public enum UserStatus
    {
        Pending,
        Active
    }
}
