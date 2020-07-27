using System;
using Tide.Core;

namespace Tide.Ork.DTOs
{
    public class RuleVaultDTO
    {
        public Guid RuleId { get; set; }
        public Guid OwnerId { get; set; }
        public string Tag { get; set; }
        public Guid KeyId { get; set; }
        public string Condition { get; set; }
        public string Action { get; set; }

        public RuleVaultDTO() { }

        public RuleVaultDTO(RuleVault rule)
        {
            RuleId = rule.RuleId;
            OwnerId = rule.OwnerId;
            Tag = rule.Tag.ToString();
            KeyId = rule.KeyId;
            Condition = rule.Condition;
            Action = rule.Action.ToString();
        }

        public RuleVault Map() 
        {
            ulong tag = ulong.TryParse(Tag, out tag) ? tag : 0;
            return new RuleVault()
            {
                RuleId = RuleId,
                OwnerId = OwnerId,
                Tag = tag,
                KeyId = KeyId,
                Condition = Condition,
                Action = (RuleAction)Enum.Parse(typeof(RuleAction), Action, true)
            };
        }

        public static implicit operator RuleVault(RuleVaultDTO r) => r.Map();
        public static implicit operator RuleVaultDTO(RuleVault r) => new RuleVaultDTO(r);
    }
}
