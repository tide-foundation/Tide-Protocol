using Tide.Encryption.Ed;

namespace Tide.Ork.Models {
    public class AddRandomResponse
    {
        public object Signature { get; set; }
        public byte[] EncryptedToken { get; set; }
        public Ed25519Point CmkPub {get ; set;}
        public Ed25519Point Cmk2Pub {get ; set;}
    }
}