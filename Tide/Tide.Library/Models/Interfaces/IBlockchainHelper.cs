using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Library.Models.Interfaces
{
    public interface IBlockchainHelper {
        // Vendor functions
        TideResponse InitializeAccount(string username);
        TideResponse ConfirmAccount(string username);

        // Ork functions
        TideResponse GetNodes(string username);
        TideResponse PostFragment(AuthenticationModel model);
        TideResponse GetFragment(AuthenticationModel model);
    }
}
