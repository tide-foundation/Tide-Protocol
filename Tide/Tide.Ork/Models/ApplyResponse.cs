namespace Tide.Ork.Models {
    public class ApplyResponse
    {
        public byte[] EncryptedRes { get; set; }
        public byte[] Token { get; set; }
        public byte[] Prism { get; set; }
    }
}