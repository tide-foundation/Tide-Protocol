using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Tide.Core;
using Tide.Simulator.Models;

namespace Tide.Simulator.Classes {
    public class BlockLayer : IBlockLayer
    {
        private readonly BlockchainContext _context;
        private readonly IHubContext<SimulatorHub> _hub;

        public BlockLayer(BlockchainContext context, IHubContext<SimulatorHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public bool Write(List<BlockData> blocks)
        {
            var writtenBlocks = new List<BlockData>();
            using (var transaction = _context.Database.BeginTransaction())
            {
                foreach (var blockData in blocks)
                {
                    try
                    {
                        var currentData = _context.Data.FirstOrDefault(d =>
                            d.Index == blockData.Index &&
                            d.Contract == blockData.Contract &&
                            d.Table == blockData.Table &&
                            d.Scope == blockData.Scope &&
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
                        writtenBlocks.Add(blockData);
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }

                transaction.Commit();

                foreach (var blockData in writtenBlocks)
                {
                    _hub.Clients.All.SendAsync("NewBlock", blockData);
                }

                return true;
            }
        }

        public bool Write(BlockData blockData)
        {
            return Write(new List<BlockData>() { blockData });
        }

        public string Read(Contract contract, Table table, string scope, string index)
        {
            var currentData = _context.Data.FirstOrDefault(d =>
                d.Index == index &&
                d.Contract == contract &&
                d.Table == table &&
                d.Scope == scope &&
                !d.Stale);

            return currentData?.Data;
        }

        public List<string> Read(Contract contract, Table table, string scope) {
            return _context.Data.Where(d =>
                d.Contract == contract &&
                d.Table == table &&
                d.Scope == scope &&
                !d.Stale).Select(d => d.Data).ToList();
        }

        public bool SetStale(Contract contract, Table table, string scope, string index)
        {
            var currentData = _context.Data.FirstOrDefault(d =>
                d.Index == index &&
                d.Contract == contract &&
                d.Table == table &&
                d.Scope == scope &&
                !d.Stale);

            if (currentData == null) return false;
            currentData.Stale = true;

            _context.SaveChanges();
            return true;
        }

        public List<BlockData> ReadHistoric(Contract contract, Table table, string scope, string index)
        {
            return _context.Data.Where(d =>
                d.Index == index &&
                d.Contract == contract &&
                d.Table == table &&
                d.Scope == scope).ToList();
        }
    }

}