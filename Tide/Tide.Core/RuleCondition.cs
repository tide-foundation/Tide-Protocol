using System;

namespace Tide.Core
{
    public class RuleCondition
    {
        public int Level { get; set; }
        public string Union { get; set; }
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public bool HashUnion => !string.IsNullOrEmpty(Union);
    }
}