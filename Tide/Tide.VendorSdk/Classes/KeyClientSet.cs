using System;
using System.Threading.Tasks;
using Tide.Core;
using System.Collections.Generic;
using System.Linq;

namespace Tide.VendorSdk.Classes
{
    public class KeyClientSet
    {
        private readonly KeyClient[] _clients;

        public KeyClientSet(IEnumerable<Uri> uris)
        {
            _clients = uris.Select(uri => new KeyClient(uri)).ToArray();
        }

        public async Task<KeyIdVault[]> Get(Guid uid)
            => await Task.WhenAll(_clients.Select(cln => cln.Get(uid)));

        public async Task SetOrUpdate(KeyIdVault key)
            => await Task.WhenAll(_clients.Select(cln => cln.SetOrUpdate(key)));
    }
}
