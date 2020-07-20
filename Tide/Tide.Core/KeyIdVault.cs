using System;
using System.Collections.Generic;
using System.Linq;
using Tide.Encryption;
using Tide.Encryption.Ecc;
using Tide.Encryption.Tools;

namespace Tide.Core
{
    public class KeyIdVault : SerializableByteBase<KeyIdVault>, IGuid
    {
        public Guid Id => KeyId;
        public Guid KeyId { get; private set; }
        public C25519Key Key { get; private set; }

        public KeyIdVault() : base(1) { }

        public KeyIdVault(C25519Key key) : base(1) { Set(key); }

        public void Set(C25519Key key) {
            Key = key;
            KeyId = new Guid(Utils.Hash(key.ToByteArray()).Take(16).ToArray());
        }

        protected override IEnumerable<byte[]> GetItems()
        {
            yield return KeyId.ToByteArray();
            yield return Key != null ? Key.ToByteArray() : new byte[]{};
        }

        protected override void SetItems(IReadOnlyList<byte[]> data)
        {
            KeyId = new Guid(data[0]);
            Key = data[1].Any() ? new C25519Key(data[1]) : null;
        }
    }
}
