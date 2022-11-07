using System;
using Tide.Core;
using Tide.Encryption.Ed;
using System.Text.Json;

namespace Tide.Ork.Models {
     public class ApplyResponseToEncrypt
    {
        public byte[] GBlurUserCMKi {get; set;}
        public byte[] GCMK2 {get; set;}
        public byte[] GCMK {get; set;}
        public byte[] CertTimei {get; set;} // 32 byte size

        //not currently being used, sits here just in case
        public byte[] ToByteArray()
        {
            var buffer = new byte[224];
            GBlurUserCMKi.CopyTo(buffer, 0); 
            GCMK2.CopyTo(buffer, 64);
            GCMK.CopyTo(buffer, 128);
            CertTimei.CopyTo(buffer, 192);
            return buffer;
        }

        // doing this because the size of ed25519 points will change in future
        public string ToJSON() => JsonSerializer.Serialize(this);


    }
}