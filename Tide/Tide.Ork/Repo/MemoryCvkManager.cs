using System;
using System.Threading.Tasks;
using Tide.Core;

namespace Tide.Ork.Repo
{
    public class MemoryCvkManager : MemoryManagerBites<CvkVault>, ICvkManager
    {
        public Task Confirm(Guid id) => Task.CompletedTask; //throw new NotImplementedException("Do not invoke confirm in memory manager");
    }
}