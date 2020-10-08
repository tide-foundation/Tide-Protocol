using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Tide.Core;
using Tide.Simulator.Models;

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
                        return (false,e.Message);
                    }
                }

                transaction.Commit();

          

                return (true,null);
            }
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
    }

}