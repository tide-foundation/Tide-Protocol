using System;
using System.Threading.Tasks;
using Tide.Core;

namespace Tide.Ork.Repo
{
    public class MemoryCvkManager : MemoryManagerBase<CvkVault>, ICvkManager
    {
        public Task Confirm(Guid id) => throw new NotImplementedException("Do not invoke confirm in memory manager");
    }
}