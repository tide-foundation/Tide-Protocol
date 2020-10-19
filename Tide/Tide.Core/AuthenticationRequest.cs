namespace Tide.Core
{
    public class AuthenticationRequest
    {
        public AuthenticationRequest() {
            // Used for deserialization
        }

        public AuthenticationRequest(string orkId, string publicKey) {
            OrkId = orkId;
            PublicKey = publicKey;
        }

        
        public string OrkId { get; set; }
        public string PublicKey { get; set; }
    }

    public class AuthenticationResponse {

        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
