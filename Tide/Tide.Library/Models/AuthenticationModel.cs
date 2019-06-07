namespace Tide.Library.Models {
    public struct AuthenticationModel {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string PublicKey { get; set; }
        public string AccountPublic { get; set; }
        public string AccountPrivateFrag { get; set; }
        public string SiteUrl { get; set; }
        public string Ip { get; set; }
    }
}