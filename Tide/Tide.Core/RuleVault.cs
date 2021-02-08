using System;
using System.Collections.Generic;
using System.Text;
using Tide.Encryption;

namespace Tide.Core
{
    public enum RuleAction { Allow = 1, Deny = 2 }

    public class RuleVault : SerializableByteBase<RuleVault>, IGuid
    {
        public Guid Id => RuleId;
        public Guid RuleId { get; set; }
        public Guid OwnerId { get; set; }
        public ulong Tag { get; set; }
        public Guid KeyId { get; set; }
        public string Condition { get; set; }

        public RuleAction Action { get; set; }
        public bool IsAllowed => Action == RuleAction.Allow;
        public bool IsDenied => Action == RuleAction.Deny;

        public RuleVault() : base(1) { 
            Condition = string.Empty;
        }

        protected override IEnumerable<byte[]> GetItems()
        {
            yield return RuleId.ToByteArray();
            yield return BitConverter.GetBytes(Tag);
            yield return KeyId.ToByteArray();
            yield return Encoding.UTF8.GetBytes(Condition);
            yield return BitConverter.GetBytes((int)Action);
            yield return OwnerId.ToByteArray();
        }

        protected override void SetItems(IReadOnlyList<byte[]> data)
        {
            RuleId = new Guid(data[0]);
            Tag = BitConverter.ToUInt64(data[1], 0); 
            KeyId = new Guid(data[2]);
            Condition = Encoding.UTF8.GetString(data[3]);
            Action = (RuleAction) BitConverter.ToInt32(data[4], 0);
            OwnerId = new Guid(data[5]);
        }
    }
}
