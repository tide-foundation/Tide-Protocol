using System;
using Tide.Core;
using Tide.Encryption.Ed;
using System.Text.Json;

namespace Tide.Ork.Models {
     public class ApplyResponseToEncrypt
    {
        public Ed25519Point GBlurUserCMKi {get; set;}
        public Ed25519Point GCMK2 {get; set;}
        public Ed25519Point GCMK {get; set;}
        public byte[] CertTimei {get; set;} // 32 byte size

        //not currently being used, sits here just in case
        public byte[] ToByteArray()
        {
            var buffer = new byte[224];
            GBlurUserCMKi.ToByteArray().CopyTo(buffer, 0); 
            GCMK2.ToByteArray().CopyTo(buffer, 64);
            GCMK.ToByteArray().CopyTo(buffer, 128);
            CertTimei.CopyTo(buffer, 192);
            return buffer;
        }

        // doing this because the size of ed25519 points will change in future
        public byte[] ToJSON() => JsonSerializer.SerializeToUtf8Bytes(this);
    }
}