namespace Tide.Ork.Models {
    public class CMKResponse
    {
        public byte[] GCMKi { get; set; }
        public byte[] YijCipher { get; set; }
        public byte[] GMultiplied1 {get; set;}
        public byte[] GMultiplied2 {get; set;}
        public string CMKtimestampi {get; set;}
    }
}