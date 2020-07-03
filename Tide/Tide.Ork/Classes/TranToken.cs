using System;
using System.Collections.Generic;
using System.Linq;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Tools;

namespace Tide.Ork.Classes {
    public class TranToken
    {
        public TranToken() { }

        public Guid Id { get; set; }
        public long Ticks { get; set; }
        public byte[] Sign { get; set; }

        public bool Check(AesKey key) => Utils.Equals(Sign, GenSign(key));

        private byte[] GenSign(AesKey key) => GenSign(key, Id, Ticks);

        public AesSherableKey GenKey(AesKey key) => AesSherableKey.Parse(key.Hash(ToByteArray()));

        public int GetByteCount() => 16 + 8 + Sign.Length;

        public override string ToString() => Convert.ToBase64String(ToByteArray());

        public byte[] ToByteArray() => Id.ToByteArray()
            .Concat(BitConverter.GetBytes(Ticks)).Concat(Sign).ToArray();

        private static byte[] GenSign(AesKey key, Guid id, long ticks) =>
            key.Hash(id.ToByteArray().Concat(BitConverter.GetBytes(ticks)).ToArray());

        public static TranToken Generate(AesKey key)
        {
            var id = Guid.NewGuid();
            var ticks = DateTime.UtcNow.Ticks;
            var sign = GenSign(key, id, ticks);

            return new TranToken { Id = id, Ticks = ticks, Sign = sign };
        }

        public static TranToken Parse(IReadOnlyList<byte> bytes)
        {
            var id = new Guid(bytes.Take(16).ToArray());
            var ticks = BitConverter.ToInt64(bytes.Skip(16).Take(8).ToArray());
            var sign = bytes.Skip(24).ToArray();

            return new TranToken { Id = id, Ticks = ticks, Sign = sign };
        }
    }
}
