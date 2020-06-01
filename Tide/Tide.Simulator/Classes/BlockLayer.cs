

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Tide.Simulator.Models;

namespace Tide.Simulator.Classes {
    public class BlockLayer : IBlockLayer {
        private static Dictionary<Tuple<Contract, Table, string, string>, BlockData> _inMemory;
        private readonly BlockchainContext _context;

        public BlockLayer(BlockchainContext context) {
            _context = context;

            if (_inMemory == null) {
                _inMemory = _context.Data.Where(d => !d.Stale).ToDictionary(d => new Tuple<Contract, Table, string, string>(d.Contract, d.Table, d.Scope, d.Index), d => d);
            }
        }

        public bool Write(Contract contract, Table table, string scope, string index, object data) {
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
                        Index = index,
                        DateCreated = DateTimeOffset.Now,
                        Stale = false,
                        Data = JsonConvert.SerializeObject(data)
                };
                    _context.Add(newData);

                    _context.SaveChanges();
                    transaction.Commit();

                    _inMemory[new Tuple<Contract, Table, string, string>(contract, table, scope, index)] = newData;

                    return true;
                }
                catch (Exception) {
                    return false;
                }
            }
        }

        public bool Read<T>(Contract contract, Table table, string scope, string index, out T data) {
            if (_inMemory.TryGetValue(new Tuple<Contract, Table, string, string>(contract, table, scope, index), out var record)) {
                data = JsonConvert.DeserializeObject<T>(record.Data);
                return true;
            }

            data = default;
            return false;
        }

    }


}