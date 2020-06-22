using System;
using System.Linq;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Tools;

namespace Tide.Ork.Classes {
    public class VerifyChallenge {
        private const long _window = TimeSpan.TicksPerSecond * 5;

        public static bool Check(AesKey key, byte[] signature, long ticks, params byte[][] message) {
            var body = message.SelectMany(msg => msg).ToArray();

            var sign = key.Hash(body);
            var check1 = Utils.Equals(sign, signature);

            var now = DateTime.UtcNow;
            var signedTime = DateTime.FromBinary(ticks);
            var check2 = signedTime >= now.AddTicks(-_window) && signedTime <= now.AddTicks(_window);

            return check1 && check2;
        }
    }
}