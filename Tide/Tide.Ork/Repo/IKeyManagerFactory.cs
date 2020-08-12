using Tide.Core;

namespace Tide.Ork.Repo {
    public interface IKeyManagerFactory {
        ICmkManager BuildCmkManager();
        ICvkManager BuildManagerCvk();
        IKeyIdManager BuildKeyIdManager();
        IRuleManager BuildRuleManager();
    }
}