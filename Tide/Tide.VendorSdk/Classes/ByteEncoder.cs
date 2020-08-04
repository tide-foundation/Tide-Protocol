using System;
using System.Numerics;
using Tide.Encryption.Ecc;
using Tide.Encryption.Tools;

namespace Tide.VendorSdk.Classes {
    public static class ByteEncoder {
        public static byte[] FromBase64Url(this string data)
        {
            return Convert.FromBase64String(data.DecodeBase64Url());
        }
    }
}