using System;
using Microsoft.AspNetCore.Mvc;
using Tide.Core;
using Tide.Simulator.Classes;

namespace Tide.Simulator.Contracts
{
    public class DnsContract : IContract
    {
        private readonly IBlockLayer _blockchain;
        private readonly object _lockObj;

        public DnsContract(IBlockLayer blockchain, object lockObj)
        {
            _blockchain = blockchain;
            _lockObj = lockObj;
        }

        public bool Matchs(Transaction transaction)
        {
            var location = transaction.Location.ToLower();
            return location.Contains("authentication") && location.Contains("dns") ;
        }

        public IActionResult Process(Transaction transaction)
        {
            var dns = DnsEntry.Parse(transaction.Data);
            if (!dns.VerifyForUId())
                return new BadRequestObjectResult("Client's signature is invalid");
            
            //TODO: Add ork signature verification
            if (false)
                return new BadRequestObjectResult("One of the ORK signatures is invalid");

            //TODO: Verify if this works
            var sourceTran = _blockchain.Read(transaction.Location, transaction.Index);
            if (sourceTran != null)
                return new BadRequestObjectResult("The account already exists");
            
            lock (_lockObj) {
                var result = _blockchain.Write(transaction);
                return result.success ? new OkResult() as IActionResult
                    :  new BadRequestObjectResult(result.error);
            }
        }
    }
}