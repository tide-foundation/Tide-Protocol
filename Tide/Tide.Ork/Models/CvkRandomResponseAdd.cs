using Tide.Encryption.Ed;

namespace Tide.Ork.Models {
    public class CvkRandomResponseAdd
    {
        public object Signature { get; set; }
        public byte[] EncryptedToken { get; set; }
        public Ed25519Point CvkPub {get ; set;}
        public Ed25519Point Cvk2Pub {get ; set;}
    
    }
}