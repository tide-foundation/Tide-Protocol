using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ContractManager> _logger;

        public ContractManager(IBlockLayer blockchain, object lockObj, Settings settings, ILogger<ContractManager> logger)
        {
            _blockchain = blockchain;
            _lockObj = lockObj;
            _settings = settings;
            _logger = logger;
        }

        private IEnumerable<IContract> Contracts() {
            yield return new DnsContract(_blockchain, _lockObj, _settings.Features, _logger);
            yield return new AuditTrailContract(_blockchain, _lockObj, _settings.Threshold, _logger);
        }

        public IActionResult Process(Transaction transaction) {
            foreach (var contract in Contracts())
            {
                if (contract.Matchs(transaction)) {
                    _logger.LogInformation("{0} is processin [location: {1} index: {2}]", nameof(contract), transaction.Location, transaction.Index);
                    return contract.Process(transaction);
                }
            }

            lock (_lockObj) {
                _logger.LogInformation("{0} is processin [location: {1} index: {2}]", nameof(ContractManager), transaction.Location, transaction.Index);
                var result = _blockchain.Write(transaction);
                return result.success ? new OkResult() as IActionResult
                    :  new BadRequestObjectResult(result.error);
            }
        }
    }
}