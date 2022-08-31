using System;
using Tide.Core;
using Tide.Encryption;
using Tide.Encryption.AesMAC;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public abstract class SimulatorManagerCipherBase<T> : SimulatorManagerBase<T> where T : SerializableByteBase<T>, IGuid, new()
    {
        protected readonly AesKey _key;
        protected virtual bool IsEncrypted => true;

        public SimulatorManagerCipherBase(string orkId, SimulatorClient client, AesKey key) : base(orkId, client) {
            _key = key;
        }

        protected override T Map(string data){
            return(!IsEncrypted ? SerializableByteBase<T>.Parse(data.Replace("\"", "")) : SerializableByteBase<T>.Parse(_key.Decrypt(data.Replace("\"", ""))));
        }


        protected override string Map(T entity) => !IsEncrypted ? entity.ToString() : _key.EncryptStr(entity);
    }
}
