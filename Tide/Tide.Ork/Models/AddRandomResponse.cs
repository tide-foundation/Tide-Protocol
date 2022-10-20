namespace Tide.Ork.Models {
    public class AddRandomResponse
    {
        public byte[] Signature { get; set; }
        public byte[] EncryptedToken { get; set; }
    }
}