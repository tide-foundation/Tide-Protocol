using Library;

namespace Tide.Simulator.Classes {
    public interface IAuthentication {
        AuthenticationResponse Register(AuthenticationRequest request);
        AuthenticationResponse Login(AuthenticationRequest request);
    }
}