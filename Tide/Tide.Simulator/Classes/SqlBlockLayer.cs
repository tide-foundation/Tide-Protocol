using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Tide.Core;

namespace Tide.Simulator.Classes {
    public class SqlBlockLayer : IBlockLayer
    {
        private readonly BlockchainContext _context;
      
        public SqlBlockLayer(BlockchainContext context)
        {
            _context = context;
        }

        public (bool success, string error) Write(Transaction transaction) {
            return Write(new List<Transaction>() { transaction });
        }

        public (bool success, string error) Write(List<Transaction> transactions) {
          
            using (var transaction = _context.Database.BeginTransaction())
            {
                foreach (var blockData in transactions)
                {
                    try
                    {
                        var currentData = _context.Transactions.FirstOrDefault(d =>
                            d.Index == blockData.Index &&
                            d.Location == blockData.Location &&
                            !d.Stale);

                        if (currentData != null)
                        {
                            currentData.Stale = true;
                            _context.Update(currentData);
                        }

                        blockData.DateCreated = DateTimeOffset.Now;
                        blockData.Stale = false;

                        _context.Add(blockData);

                        _context.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        return (false,e.InnerException.Message);
                    }
                }

                transaction.Commit();

          

                return (true,null);
            }
        }

        public Transaction Read(string location, string index) {
            return _context.Transactions.FirstOrDefault(d => d.Index == index
                && d.Location == location && !d.Stale);
        }

        public Transaction Read(string contract, string table, string scope, string index) {
            return _context.Transactions.FirstOrDefault(d =>
                d.Index == index &&
                d.Location == Transaction.CreateLocation(contract, table, scope) &&
                !d.Stale);
        }

        public List<Transaction> Read(string contract, string table, string scope) {
            return _context.Transactions.Where(d =>
                d.Location == Transaction.CreateLocation(contract, table, scope) &&
                !d.Stale).ToList();
        }

        public List<Transaction> Read(string contract, string table, string scope, KeyValuePair<string, string> index) {
            throw new NotImplementedException();
        }

        public bool SetStale(string contract, string table, string scope, string index) {
            var currentData = _context.Transactions.FirstOrDefault(d =>
                d.Index == index &&
                d.Location == Transaction.CreateLocation(contract, table, scope) &&
                !d.Stale);

            if (currentData == null) return false;
            currentData.Stale = true;

            _context.SaveChanges();
            return true;
        }

        public List<Transaction> ReadHistoric(string contract, string table, string scope, string index) {
            return _context.Transactions.Where(d =>
                d.Index == index &&
                d.Location == Transaction.CreateLocation(contract, table, scope)).ToList();
        }

        public List<Transaction> ReadHistoric() {
            return _context.Transactions.ToList();
        }

        public List<AuthStateTran> SelectPendingLogs(IEnumerable<Guid> ids, int threshold)
        {
            var pending = new List<AuthStateTran>();
            if (!ids.Any())
                return pending;

            var prmSql = string.Join(',', ids.Select((_, i) => $"@p{i}"));
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText =
                @"select TranId, Successful, count(Successful) cnt, string_agg(OrkId, '|') orks 
                from AuthPendings
                where TranId in (" + prmSql + @")
                group by Successful, TranId";

                foreach (var prm in ids.Select((id, i) => new SqlParameter($"@p{i}", id)))
                {
                    command.Parameters.Add(prm);    
                }

                _context.Database.OpenConnection();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        pending.Add(new AuthStateTran
                        {
                            TranId = result.GetGuid(0),
                            Successful = result.GetBoolean(1),
                            Count = result.GetInt32(2),
                            Orks = result.GetString(3),
                        });
                    }
                }
            }
            return pending;
        }

        public List<Auth> SelectLogs(IEnumerable<Guid> ids)
        {
            return _context.Auths.Where(auth =>  ids.Contains(auth.TranId)).ToList();
        }

        public void UpdateLogs(IEnumerable<Auth> logs)
        {

            foreach (var log in logs)
            {
                bool tracking = _context.ChangeTracker.Entries<Auth>().Any(t => t.Entity.Id == log.Id);
                if (!tracking)
                {
                    _context.Auths.Update(log);
                }
            }

            _context.SaveChanges();
        }

        public void InsertLogs(IEnumerable<Auth> logs)
        {
            _context.Auths.AddRange(logs);
            _context.SaveChanges();
        }

        public void InsertPendingLogs(IEnumerable<AuthPending> logs)
        {
            _context.AuthPendings.AddRange(logs);
            _context.SaveChanges();
        }
    }
}