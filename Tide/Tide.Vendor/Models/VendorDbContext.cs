using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.VendorSdk.Classes;

namespace Tide.Vendor.Models
{
    public class VendorDbContext
    {
        private readonly ConcurrentDictionary<Guid, ApplicationUser> _items;

        public VendorDbContext()
        {
            _items = new ConcurrentDictionary<Guid, ApplicationUser>();
        }

        public bool CreateApplicationUser(ApplicationUser user) {
            _items[user.Vuid] = user;

            return true;
        }

        public ApplicationUser GetAccount(Guid vuid) {
            if (!_items.ContainsKey(vuid))return null;

            return _items[vuid];
        }
    }
}
