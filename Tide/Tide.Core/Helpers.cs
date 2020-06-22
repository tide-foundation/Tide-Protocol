using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Tide.Encryption.Tools;

namespace Tide.Core
{
    public static class Helpers
    {
        public static  byte[] FromBase64(string input)
        {
            return Convert.FromBase64String(input.DecodeBase64Url());
        }

        public static Guid GetTideId(string user)
        {
            return new Guid(FromBase64(user));
        }

        public static BigInteger GetBigInteger(string number)
        {
            return new BigInteger(FromBase64(number), true, true);
        }
    }
}
