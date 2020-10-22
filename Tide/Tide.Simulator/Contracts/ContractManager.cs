using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tide.Core;
using Tide.Simulator.Classes;

namespace Tide.Simulator.Contracts
{
    public class ContractManager
    {
        private readonly IBlockLayer _blockchain;
        private readonly object _lockObj;

        public ContractManager(IBlockLayer blockchain, object lockObj)
        {
            _blockchain = blockchain;
            _lockObj = lockObj;
        }

        private IEnumerable<IContract> Contracts() {
            yield return new DnsContract(_blockchain, _lockObj);
        }

        public IActionResult Process(Transaction transaction) {
            foreach (var contract in Contracts())
            {
                if (contract.Matchs(transaction))
                    return contract.Process(transaction);
            }

            lock (_lockObj) {
                var result = _blockchain.Write(transaction);
                return result.success ? new OkResult() as IActionResult
                    :  new BadRequestObjectResult(result.error);
            }
        }
    }
}