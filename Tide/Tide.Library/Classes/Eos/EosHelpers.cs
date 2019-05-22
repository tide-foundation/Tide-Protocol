using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Tide.Library.Classes.Eos
{
    public static class EosHelpers
    {
        public const string InitializeAccount = "inituser";
        public const string ConfirmAccount = "confirmuser";
        public const string AddUser = "add_frag";
        public static ulong ConvertToUint64(this string input, bool needToHash = true)
        {
            var hashed = needToHash ? Sha256(input) : input;
            var bytes = Encoding.UTF8.GetBytes(hashed);

            return BitConverter.ToUInt64(bytes, 0);

            string Sha256(string value) {
                using var hash = SHA256.Create();
                return string.Concat(hash
                    .ComputeHash(Encoding.UTF8.GetBytes(value))
                    .Select(item => item.ToString("x2")));
            }
        }

        public static int GetEpoch() {
            var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return (int)t.TotalSeconds;
        }
    }
}
