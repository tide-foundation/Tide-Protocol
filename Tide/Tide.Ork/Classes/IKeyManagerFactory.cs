using Tide.Core;

namespace Tide.Ork.Classes {
    public interface IKeyManagerFactory {
        IKeyManager BuildManager();
        IManager<CvkVault> BuildManagerCvk();
    }
}