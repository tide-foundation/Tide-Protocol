// 
// Tide Protocol - Infrastructure for the Personal Data economy
// Copyright (C) 2019 Tide Foundation Ltd
// 
// This program is free software and is subject to the terms of 
// the Tide Community Open Source License as published by the 
// Tide Foundation Limited. You may modify it and redistribute 
// it in accordance with and subject to the terms of that License.
// This program is distributed WITHOUT WARRANTY of any kind, 
// including without any implied warranty of MERCHANTABILITY or 
// FITNESS FOR A PARTICULAR PURPOSE.
// See the Tide Community Open Source License for more details.
// You should have received a copy of the Tide Community Open 
// Source License along with this program.
// If not, see https://tide.org/licenses_tcosl-1-0-en
//

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Tide.Library.Classes.Eos {
    public static class EosHelpers {
        public const string InitializeAccount = "inituser";
        public const string ConfirmAccount = "confirmuser";
        public const string CreateVendor = "addvendor";

        public static ulong ConvertToUint64(this string input, bool needToHash = true) {
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
            return (int) t.TotalSeconds;
        }
    }
}