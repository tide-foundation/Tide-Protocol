using System;
using Tide.Core;
using Tide.Encryption;
using Tide.Encryption.AesMAC;
using Tide.Ork.Classes;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace Tide.Ork.Repo {
    public abstract class SimulatorManagerCipherBase<T> : SimulatorManagerBase<T> where T : SerializableByteBase<T>, IGuid, new()
    {
        protected readonly AesKey _key;
        protected virtual bool IsEncrypted => true;

        public SimulatorManagerCipherBase(string orkId, SimulatorClient client, AesKey key) : base(orkId, client) {
            _key = key;
        }

        protected override T Map(string data){
            Console.WriteLine(DeserializeBytes(_key.Decrypt(data.Replace("\"", ""))));
            return(!IsEncrypted ? SerializableByteBase<T>.Parse(data.Replace("\"", "")) : SerializableByteBase<T>.Parse(_key.Decrypt(data.Replace("\"", ""))));
        }

        public static List<byte[]> DeserializeBytes(IReadOnlyList<byte> bytes, int countSize = 1)
        {
            if (bytes == null || bytes.Count < 1 || (bytes[0] == 0 && bytes.Count > 1)){
                Console.WriteLine("1");
                throw new FormatException();
            }
            

            var numItems = bytes[0];
            if (numItems == 0)
                return new List<byte[]>();

            var lenIdx = 1;
            var numbers = new List<byte[]>(numItems);
            for (int i = 0; i < numItems; i++)
            {
                if (lenIdx >= bytes.Count || lenIdx + countSize > bytes.Count){
                    Console.WriteLine("2");
                    throw new FormatException();
                }

                var nextBytes = ToInt(bytes.Skip(lenIdx).Take(countSize));

                if (lenIdx + countSize + nextBytes > bytes.Count){
                    Console.WriteLine("3");
                    throw new FormatException();
                }

                numbers.Add(bytes.Skip(lenIdx + countSize).Take(nextBytes).ToArray());
                lenIdx += nextBytes + countSize;
            }

            if (lenIdx != bytes.Count){
                Console.WriteLine("4");
                throw new FormatException();
            }

            return numbers;
        }
        private static int ToInt(IEnumerable<byte> bytes)
        {
            return (int)new BigInteger(bytes.ToArray(), true, true);
        }

        protected override string Map(T entity) => !IsEncrypted ? entity.ToString() : _key.EncryptStr(entity);
    }
}
