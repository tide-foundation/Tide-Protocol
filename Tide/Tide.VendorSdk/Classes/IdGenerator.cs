using System;
using System.Linq;
using System.Numerics;
using System.Text;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Tools;

namespace Tide.VendorSdk.Classes
{
    public class IdGenerator
    {
        public Guid Guid { get; }

        public BigInteger Id { get => new BigInteger(ToByteArray(), true, true); }

        public ulong Long { get => BitConverter.ToUInt64(ToByteArray().Take(8).ToArray()); }

        public IdGenerator(Guid guid)
        {
            Guid = guid;
        }

        public byte[] ToByteArray() => this.Guid.ToByteArray();

        public static IdGenerator Seed(Uri uri, AesKey key = null) => Seed(uri.Authority, key);

        public static IdGenerator Seed(string data, AesKey key = null) => Seed(Encoding.UTF8.GetBytes(data), key);

        public static IdGenerator Seed(byte[] data, AesKey key = null)
        {
            if (key != null)
                return new IdGenerator(new Guid(key.Hash(data).Take(16).ToArray()));

            return new IdGenerator(new Guid(Utils.Hash(data).Take(16).ToArray()));
        }
    }
}