using Tide.Core;
using Tide.Simulator.Models;

namespace Tide.Simulator.Classes {
    public interface IAuthentication {
        (bool success, string error) Register(AuthenticationRequest request);
        Account GetAccount(string orkId);
    }
}