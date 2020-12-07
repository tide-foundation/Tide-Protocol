using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tide.Core;
using Tide.Simulator.Classes;

namespace Tide.Simulator.Contracts
{
    public class AuditTrailContract : IContract
    {
        private static readonly object lockDns;

        private readonly IBlockLayer _blockchain;
        private readonly object _lockObj;
        private int _threshold;
        private readonly ILogger _logger;

        static AuditTrailContract() => lockDns = new Object();

        public AuditTrailContract(IBlockLayer blockchain, object lockObj, int threshold, ILogger logger)
        {
            _blockchain = blockchain;
            _lockObj = lockObj;
            _threshold = threshold;
            _logger = logger;
        }

        public bool Matchs(Transaction transaction)
        {
            return transaction.Index.ToLower().Contains("audittrail");
        }

        private List<AuthPending> Parse(string text) {
            using (StringReader reader = new StringReader(text))
            {
                string line;
                var auths = new List<AuthPending>();
                while ((line = reader.ReadLine()) != null)
                {
                    auths.Add(AuthPending.Parse(line));
                }
                return auths;
            }
        }
        
        public IActionResult Process(Transaction transaction)
        {
            try
            {
                lock (lockDns)
                {
                    var auths = Parse(transaction.Data);
                    var ids = auths.Select(log => log.TranId).ToList();
                    using (_logger.BeginScope(ids))
                    {
                        if (!ids.Any())
                            return new OkResult();

                        var pendingLogs = _blockchain.SelectPendingLogs(ids, _threshold - 1);
                        var toUpdate = _blockchain.SelectLogs(ids);
                        var toUpdateIds = toUpdate.Select(log => log.TranId);

                        foreach (var log in toUpdate)
                        {
                            var pending = auths.SingleOrDefault(l => l.TranId == log.TranId);
                            if (pending.Successful)
                                log.SuccessfulOrks += "|" + pending.OrkId;
                            else
                                log.UnsuccessfulOrks += "|" + pending.OrkId;
                        }

                        var toInsert =
                            (from p in pendingLogs.Where(log => log.Count >= _threshold - 1 && !toUpdateIds.Contains(log.TranId))
                             join n in auths on new { p.TranId, p.Successful } equals new { n.TranId, n.Successful }
                             where p.Count + 1 >= _threshold
                             select new Auth()
                             {
                                 TranId = p.TranId,
                                 Successful = p.Successful,
                                 Method = n.Method,
                                 Uid = n.Uid,
                                 SuccessfulOrks = p.Successful ? p.Orks + "|" + n.OrkId : string.Empty,
                                 UnsuccessfulOrks = !p.Successful ? p.Orks + "|" + n.OrkId : string.Empty,
                                 Time = DateTimeOffset.UtcNow
                             }).ToList();

                        foreach (var log in toInsert)
                        {
                            var pending = pendingLogs.SingleOrDefault(l => l.TranId == log.TranId && l.Successful != log.Successful);
                            if (pending == null)
                                continue;

                            if (pending.Successful)
                                log.SuccessfulOrks = pending.Orks;
                            else
                                log.UnsuccessfulOrks = pending.Orks;
                        }

                        var toPending = auths.Where(auth => !toInsert.Any(ins => ins.TranId == auth.TranId) && !toUpdate.Any(itm2 => itm2.TranId == auth.TranId)).ToList();

                        _logger.LogInformation($"To update: {string.Join(' ', toUpdate.Select(trn => trn.Id))}");
                        _logger.LogInformation($"To insert: {string.Join(' ', toInsert.Select(trn => trn.Id))}");

                        //TODO: add transaction in database
                        //TODO: delete or change pending processing status and use toPending instead of auths
                        _blockchain.UpdateLogs(toUpdate);
                        _blockchain.InsertLogs(toInsert);
                        _blockchain.InsertPendingLogs(auths);

                        return new OkResult();
                    }
                }
            }
            catch (System.Exception e)
            {
                _logger.LogError(0, e, "Error processing transaction: {0}", transaction.Data);
                return new ObjectResult(e.Message) { StatusCode = 500 };
            }
        }
    }
}