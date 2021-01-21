using Tide.Ork.Classes.Rules;

namespace Tide.Core
{
    public static class RuleExtention
    {
        public static bool Eval(this RuleVault rule) {
            return new RuleConditionEval(rule).Run();
        }
    }
}