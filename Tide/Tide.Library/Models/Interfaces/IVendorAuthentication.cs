using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Library.Models.Interfaces
{
    public interface IVendorAuthentication {
        TideResponse InitializeAccount(AuthenticationModel model);
        TideResponse ConfirmAccount(AuthenticationModel model);
    }
}
