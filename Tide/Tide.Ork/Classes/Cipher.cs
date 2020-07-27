using System;
using System.Linq;
using Tide.Encryption.Ecc;
using Tide.Encryption.EcDSA;

namespace Tide.Ork.Classes {
    public class Cipher {
        private static int TagSize => 8;
        private static int EncryptionSize => 32 * 3;
        private static int AsymmetricSize => EncryptionSize * 2 + TagSize;

        public static bool CheckAsymmetric(byte[] data, C25519Key key) {
            if (data == null || data.Length != AsymmetricSize)
                return false;

            var cipher = data.Take(EncryptionSize + TagSize).ToArray();
            var signature = data.Skip(EncryptionSize + TagSize).ToArray();

            return C25519Dsa.Verify(cipher, signature, key);
        }

        public static ulong GetTag(byte[] data)
        {
            var tag = data.Skip(EncryptionSize).Take(TagSize).ToArray();
            return BitConverter.ToUInt64(tag, 0);
        }

        public static C25519Point GetCipherC1(byte[] dataBuffer)
        {
            return C25519Cipher.Parse(dataBuffer.Take(EncryptionSize).ToArray()).C1;
        }
    }
}