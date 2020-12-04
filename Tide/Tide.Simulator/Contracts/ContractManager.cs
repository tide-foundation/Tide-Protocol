using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tide.Core;
using Tide.Simulator.Classes;
using Tide.Simulator.Models;

namespace Tide.Simulator.Contracts
{
    public class ContractManager
    {
        private readonly IBlockLayer _blockchain;
        private readonly object _lockObj;
        private readonly Settings _settings;

        public ContractManager(IBlockLayer blockchain, object lockObj, Settings settings)
        {
            _blockchain = blockchain;
            _lockObj = lockObj;
            _settings = settings;
        }

        private IEnumerable<IContract> Contracts() {
            yield return new DnsContract(_blockchain, _lockObj);
            yield return new AuditTrailContract(_blockchain, _lockObj, _settings.Threshold);
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