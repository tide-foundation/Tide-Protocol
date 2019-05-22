using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Tide.Library.Classes.Eos
{
    public static class EosHelpers
    {
        public const string InitializeAccount = "init";
        public const string FinalizeAccount = "finalize";
        public const string AddUser = "adduser";
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
    }
}
