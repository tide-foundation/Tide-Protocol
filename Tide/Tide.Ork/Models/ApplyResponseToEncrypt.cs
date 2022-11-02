using System;
using Tide.Core;
using Tide.Encryption.Ed;
using System.Text.Json;

namespace Tide.Ork.Models {
     public class ApplyResponseToEncrypt
    {
        public Ed25519Point GBlurUserCMKi {get; set;}
        public Ed25519Point GBlindR {get; set;}
        public Ed25519Point GCMK {get; set;}
        public byte[] CertTime {get; set;} // 32 byte size

        //not currently being used, sits here just in case
        public byte[] ToByteArray()
        {
            var buffer = new byte[224];
            GBlurUserCMKi.ToByteArray().CopyTo(buffer, 0);
            GBlindR.ToByteArray().CopyTo(buffer, 64);
            GCMK.ToByteArray().CopyTo(buffer, 128);
            CertTime.CopyTo(buffer, 192);
            return buffer;
        }

        // doing this because the size of ed25519 points will change in future
        public byte[] ToJSON() => JsonSerializer.SerializeToUtf8Bytes(this);
    }
}