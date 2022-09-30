using System;
using System.Linq;
using System.Numerics;
using System.Text;
using Tide.Encryption.AesMAC;
using Tide.Encryption.Ecc;
using Tide.Encryption.Ed;
using Tide.Encryption.Tools;

namespace Tide.VendorSdk.Classes {
    public class Cipher {
        /*cipher + signature + tag*/
        private static int TagSize => 8;
        private static int EncryptionSize => 32 * 3;
        private static int AsymmetricSize => EncryptionSize * 2 + TagSize;

        public static byte[] Encrypt(string buffer, ulong tag, Tide.Encryption.Ed.Ed25519Key key)
            => Encrypt(Encoding.UTF8.GetBytes(buffer), tag, key);

        public static byte[] Encrypt(byte[] buffer, ulong tag, Tide.Encryption.Ed.Ed25519Key key)
        {
            var toAsymmetricEncrypt =  Pad32(buffer);
            var bufferSymmetric = new byte[0];

            if (buffer.Length > 32)
            {
                var secret = new AesSherableKey();
                bufferSymmetric = secret.Encrypt(buffer);
                toAsymmetricEncrypt = secret.ToByteArray();
            }
            
            var bufferAsymmetric = key.Encrypt(toAsymmetricEncrypt).ToByteArray();
            var tagBuffer = BitConverter.GetBytes(tag);
            var signature = key.EdDSASign(bufferAsymmetric.Concat(tagBuffer).ToArray()).PadLeft(32 * 3);

            var size =
              bufferAsymmetric.Length +
              tagBuffer.Length +
              signature.Length +
              bufferSymmetric.Length;
            var dimension = DimensionBuffer(size);

            var all = new byte[1 + dimension.Length + size];

            all[0] = 1; // version #

            dimension.CopyTo(all, 1);
            var step = dimension.Length + 1;

            bufferAsymmetric.CopyTo(all, step);
            step += bufferAsymmetric.Length;

            tagBuffer.CopyTo(all, step);
            step += tagBuffer.Length;

            signature.CopyTo(all, step);
            step += signature.Length;

            bufferSymmetric.CopyTo(all, step);
            return all;
        }

        public static byte[] Decrypt(byte[] data, Tide.Encryption.Ed.Ed25519Key key)
        {
            var size = data[1] & 127;
            var sizeLength = 0;
            var hasFieldSize = (data[1] & 128) != 0;
            if (hasFieldSize)
            {
                sizeLength = size;
                size = (int) new BigInteger(data.Skip(2).Take(sizeLength).ToArray(), true, true);
            }
            var step = 2 + sizeLength;

            var asymmetricCipher = data.Skip(step).Take(32 * 3).ToArray();
            
            var asymmetricPlain = key.Decrypt(asymmetricCipher);
            step += AsymmetricSize;

            if (size == 200)
                return UnPad32(asymmetricPlain);

            var symmetricKey = AesSherableKey.Parse(asymmetricPlain);
            return symmetricKey.Decrypt(data.Skip(step).ToArray());
        }

        public static byte[] Asymmetric(byte[] data)
        {
            var step = HeadEnd(data);
            return data.Skip(step).Take(AsymmetricSize).ToArray();
        }

        public static byte[] Symmetric(byte[] data)
        {
            var step = HeadEnd(data);
            step += AsymmetricSize;
            
            return data.Skip(step).ToArray();
        }

        public static C25519Cipher CipherFromAsymmetric(byte [] data)
        {
            return C25519Cipher.Parse(data.Take(32 * 3).ToArray());
        }

        public static bool CheckAsymmetric(byte[] data, Tide.Encryption.Ed.Ed25519Key key)
        {
            if (data == null || data.Length != AsymmetricSize)
                return false;

            var cipher = data.Take(EncryptionSize + TagSize).ToArray();
            var signature = data.Skip(EncryptionSize + TagSize).ToArray();

            return key.EdDSAVerify(cipher, signature);
        }
 
        private static int HeadEnd(byte[] data)
        {
            var sizeLength = (data[1] & 128) != 0 ? data[1] & 127 : 0;
            return 2 + sizeLength;
        }

        private static byte[] DimensionBuffer(int size)
        {
            var buffer = (new BigInteger(size)).ToByteArray(true, true);
            return buffer.Length == 1 && buffer[0] < 128 ? buffer
              : (new[] { (byte) ((1 << 7) | buffer.Length) }).Concat(buffer).ToArray();
        }

        public static byte[] Pad32(byte[] data)
        {
            if (data.Length >= 32)
                return data;

            var buffer = new byte[32];
            data.CopyTo(buffer, 32 - data.Length);
            return buffer;
        }

        public static byte[] UnPad32(byte[] data)
        {
            var skip = 0;
            for (var i = 0; i < data.Length; i++)
            {
                if (data[i] != 0)
                    break;
                skip++;
            }

            return skip > 0 ? data.Skip(skip).ToArray() : data;
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