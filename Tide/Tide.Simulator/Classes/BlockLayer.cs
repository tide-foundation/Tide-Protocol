using System;
using System.Linq;
using Tide.Simulator.Models;

namespace Tide.Simulator.Classes {
    public class BlockLayer : IBlockLayer {
        private readonly BlockchainContext _context;

        public BlockLayer(BlockchainContext context) {
            _context = context;
        }

        public bool Write(Contract contract, Table table, string scope, string index, string data) {
            using (var transaction = _context.Database.BeginTransaction()) {
                try {
                    var currentData = _context.Data.FirstOrDefault(d =>
                        d.Index == index &&
                        d.Contract == contract &&
                        d.Table == table &&
                        d.Scope == scope &&
                        !d.Stale);

                    if (currentData != null) {
                        currentData.Stale = true;
                        _context.Update(currentData);
                    }

                    var newData = new BlockData {
                        Contract = contract,
                        Table = table,
                        Scope = scope,
                        Index = index,
                        DateCreated = DateTimeOffset.Now,
                        Stale = false,
                        Data = data
                };
                    _context.Add(newData);

                    _context.SaveChanges();
                    transaction.Commit();

                    return true;
                }
                catch (Exception) {
                    return false;
                }
            }
        }

        public string Read(Contract contract, Table table, string scope, string index) {
            var currentData = _context.Data.FirstOrDefault(d =>
                d.Index == index &&
                d.Contract == contract &&
                d.Table == table &&
                d.Scope == scope &&
                !d.Stale);

            return currentData?.Data;
        }
    }
}