using System;
using System.Linq;
using System.Numerics;
using Tide.Encryption.Tools;

namespace Tide.Ork.Classes {
    public class OrkShare {
        public BigInteger Id { get; set; }
        public BigInteger Share { get; set; }

        public OrkShare() { }

        public OrkShare(BigInteger id, BigInteger share)
        {
            Id = id;
            Share = share;
        }

        public override string ToString() => Convert.ToBase64String(ToArray());

        public byte[] ToArray() =>  Id.ToByteArray(true, true).PadLeft(16)
            .Concat(Share.ToByteArray(true, true).PadLeft(32)).ToArray();

    }
}