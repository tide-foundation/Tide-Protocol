using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tide.Library.Classes.Eos;

namespace Tide.BlockchainSimulator.Classes
{
    public class ContractLayer : IContractLayer
    {
        private IBlockLayer _blockLayer;

        public ContractLayer(IBlockLayer blockLayer) {
            _blockLayer = blockLayer;
        }

        public void CreateUser(string username) {
            var tideUser = new User();
        }
    }
}
