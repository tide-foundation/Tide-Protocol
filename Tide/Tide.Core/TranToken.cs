using System;
using System.Collections.Generic;
using System.Linq;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Tools;

namespace Tide.Core {
    public class TranToken
    {
        public TranToken() { }

        public ulong Id { get; set; }
        public long Ticks { get; set; }
        public byte[] Sign { get; set; }

        public bool Check(AesKey key) => Utils.Equals(Sign, GenSign(key));

        private byte[] GenSign(AesKey key) => GenSign(key, Id, Ticks);

        public AesSherableKey GenKey(AesKey key) => AesSherableKey.Parse(key.Hash(ToByteArray()));

        public int GetByteCount() => 8 + 8 + 16;

        public override string ToString() => Convert.ToBase64String(ToByteArray());

        public byte[] ToByteArray() => BitConverter.GetBytes(Id)
            .Concat(BitConverter.GetBytes(Ticks)).Concat(Sign).ToArray();

        private static byte[] GenSign(AesKey key, ulong id, long ticks) =>
            key.Hash(BitConverter.GetBytes(id)
            .Concat(BitConverter.GetBytes(ticks)).ToArray())
            .Take(16).ToArray();

        public static TranToken Generate(AesKey key)
        {
            var id = BitConverter.ToUInt64(Guid.NewGuid().ToByteArray().Take(8).ToArray());
            var ticks = DateTime.UtcNow.Ticks;
            var sign = GenSign(key, id, ticks);

            return new TranToken { Id = id, Ticks = ticks, Sign = sign };
        }

        public static TranToken Parse(IReadOnlyList<byte> bytes)
        {
            var id = BitConverter.ToUInt64(bytes.Take(8).ToArray());
            var ticks = BitConverter.ToInt64(bytes.Skip(8).Take(8).ToArray());
            var sign = bytes.Skip(16).ToArray();

            return new TranToken { Id = id, Ticks = ticks, Sign = sign };
        }
    }
}
