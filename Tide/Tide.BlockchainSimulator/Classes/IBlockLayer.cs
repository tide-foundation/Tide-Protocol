using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tide.BlockchainSimulator.Models;
using Tide.Library.Models;

namespace Tide.BlockchainSimulator.Classes
{
    public interface IBlockLayer {
        bool Write(JsonData data);
        bool Read(string index, out JsonData data);
    }
}
