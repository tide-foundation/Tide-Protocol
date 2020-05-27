using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Tide.BlockchainSimulator.Models;

namespace Tide.BlockchainSimulator.Classes {
    public class BlockLayer : IBlockLayer {
        private readonly BlockchainContext _context;

        private static Dictionary<string, JsonData> _inMemory;

        public BlockLayer(BlockchainContext context) {
            _context = context;

            if (_inMemory == null) {
                _inMemory = _context.Data.Where(d => !d.Stale).ToDictionary(d => d.Index, d => d);
            }
        }

        public bool Write(JsonData data) {
            if (data.Contract == Contract.Unset || data.Table == Table.Unset) {
                return false;
            }

            using (var transaction = _context.Database.BeginTransaction()) {
                try {
                    var currentData = _context.Data.FirstOrDefault(d => d.Index == data.Index && !d.Stale);

                    if (currentData != null) {
                        currentData.Stale = true;
                        _context.Update(currentData);
                    }

                    data.DateCreated = DateTimeOffset.Now;
                    data.Stale = false;
                    _context.Add(data);

                    _context.SaveChanges();
                    transaction.Commit();

                    _inMemory[data.Index] = data;

                    return true;
                }
                catch (Exception) {
                    return false;
                }
            }
        }

        public bool Read(string dataIndex, out JsonData data) {
            return _inMemory.TryGetValue(dataIndex, out data);
        }

        public static T Deserialize<T>(string data) {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static string Serialize(JsonData data) {
            return JsonConvert.SerializeObject(data);
        }
    }
}