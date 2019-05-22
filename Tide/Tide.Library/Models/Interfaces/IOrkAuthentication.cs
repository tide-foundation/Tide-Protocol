using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Library.Models.Interfaces
{
    public interface IOrkAuthentication
    {
        TideResponse GetNodes(AuthenticationModel model);
        TideResponse PostFragment(AuthenticationModel model);
        TideResponse GetFragment(AuthenticationModel model);
    }
}