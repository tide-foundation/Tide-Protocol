using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tide.Core;
using Tide.Simulator.Classes;

namespace Tide.Simulator.Contracts
{
    public class DnsContract : IContract
    {
        private readonly IBlockLayer _blockchain;
        private readonly object _lockObj;
        private readonly ILogger _logger;

        public DnsContract(IBlockLayer blockchain, object lockObj, ILogger logger)
        {
            _blockchain = blockchain;
            _lockObj = lockObj;
            _logger = logger;
        }

        public bool Matchs(Transaction transaction)
        {
            var location = transaction.Location.ToLower();
            return location.Contains("authentication") && location.Contains("dns") ;
        }

        public IActionResult Process(Transaction transaction)
        {
            try
            {
                var dns = DnsEntry.Parse(transaction.Data);
                if (!dns.VerifyForUId()) {
                    _logger.LogInformation("Invalid client's signature for {0}", transaction.Index);
                    return new BadRequestObjectResult("Client's signature is invalid");
                }

                //TODO: Add ork signature verification
                if (false) {
                    _logger.LogInformation("Invalid ORK's signature for {0}", transaction.Index);
                    return new BadRequestObjectResult("One of the ORK signatures is invalid");
                }

                //TODO: Verify if this works
                var sourceTran = _blockchain.Read(transaction.Location, transaction.Index);
                if (sourceTran != null) {
                    _logger.LogInformation("Dns entry is already created for {0}", transaction.Index);
                    return new BadRequestObjectResult("The account already exists");
                }

                lock (_lockObj)
                {
                    var result = _blockchain.Write(transaction);
                    _logger.LogInformation("Dns entry created for {0}", transaction.Index);
                    return result.success ? new OkResult() as IActionResult
                        : new BadRequestObjectResult(result.error);
                }
            }
            catch (System.Exception e)
            {
                _logger.LogError(0, e, "Error processing dns contract: {0}", transaction.Data);
                return new ObjectResult(e.Message) { StatusCode = 500 };
            }
        }
    }
}