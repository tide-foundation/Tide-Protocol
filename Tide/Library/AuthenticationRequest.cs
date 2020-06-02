namespace Library
{
    public class AuthenticationRequest
    {
        public AuthenticationRequest() {
            // Used for deserialization
        }

        public AuthenticationRequest(string username, string password) {
            Username = username;
            Password = password;
        }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AuthenticationResponse {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Error { get; set; }
    }
}
