using System;
using System.Collections.Generic;
using System.Linq;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Tools;

namespace Tide.Core {
    public class TranToken
    {
        private const long _window = TimeSpan.TicksPerHour;

        public ulong Id { get; set; }
        public long Ticks { get; set; }
        public byte[] Signature { get; set; }
        public byte[] CertTime {get; set;}

        public DateTime Time => DateTime.FromBinary(Ticks);

        public bool OnTime => Time >= DateTime.UtcNow.AddTicks(-_window)
            && Time <= DateTime.UtcNow.AddTicks(_window);

        public TranToken()
        {
            Id = BitConverter.ToUInt64(Guid.NewGuid().ToByteArray().Take(8).ToArray());
            Ticks = DateTime.UtcNow.Ticks;
            Signature = new byte[0];
        }

        public void Sign(AesKey key, byte[] data = null) => Signature = GenSign(key, data);

        public bool Check(AesKey key, byte[] data = null) => Utils.Equals(Signature, GenSign(key, data));

        private byte[] GenSign(AesKey key, byte[] data = null) => GenSign(key, Id, Ticks, data);

        public AesSherableKey GenKey(AesKey key) => AesSherableKey.Parse(key.Hash(ToByteArray()));

        public int GetByteCount() => 8 + 8 + 16;

        public override string ToString() => Convert.ToBase64String(ToByteArray());

        public byte[] ToByteArray()
        {
            var buffer = new byte[32];

            BitConverter.GetBytes(Id).CopyTo(buffer, 0);
            BitConverter.GetBytes(Ticks).CopyTo(buffer, 8);
            Signature.CopyTo(buffer, 16);

            return buffer;
        }

        private static byte[] GenSign(AesKey key, ulong id, long ticks, byte[] data = null) =>
            key.Hash(BitConverter.GetBytes(id)
                .Concat(BitConverter.GetBytes(ticks))
                .Concat(data ?? new byte[0]).ToArray())
                    .Take(16).ToArray();

        public static TranToken Generate(AesKey key, byte[] data = null)
        {
            var token = new TranToken();
            token.Sign(key, data);
            return token;
        }

        public static TranToken Parse(IReadOnlyList<byte> bytes)
        {
            if (bytes == null || bytes.Count != 32)
                return null;
            
            var id = BitConverter.ToUInt64(bytes.Take(8).ToArray());
            var ticks = BitConverter.ToInt64(bytes.Skip(8).Take(8).ToArray());
            var signature = bytes.Skip(16).ToArray();

            return new TranToken { Id = id, Ticks = ticks, Signature = signature };
        }
    }
}
