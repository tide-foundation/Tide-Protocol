using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Tide.Encryption.Tools;

namespace Tide.Core
{
    public static class Helpers
    {
        private static readonly Regex _rxBase64 = new Regex("^([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)?$", RegexOptions.Compiled);
        private static readonly Regex _rxBase64Url = new Regex("^([A-Za-z0-9-_]{4})*([A-Za-z0-9-_]{3}|[A-Za-z0-9-_]{2})?$", RegexOptions.Compiled);

        public static bool IsBase64(string data) => _rxBase64.IsMatch(data);
        public static bool IsBase64Url(string data) => _rxBase64Url.IsMatch(data);

        public static bool TryFromBase64String(string data, out byte[] bytes)
        {
            if (IsBase64(data))
            {
                bytes = Convert.FromBase64String(data);
                return true;
            }

            if (IsBase64Url(data))
            {
                string decoded = data.Replace('_', '/').Replace('-', '+');
                switch (decoded.Length % 4)
                {
                    case 2: decoded += "=="; break;
                    case 3: decoded += "="; break;
                }

                bytes = Convert.FromBase64String(decoded);
                return true;
            }

            bytes = null;
            return false;
        }

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
