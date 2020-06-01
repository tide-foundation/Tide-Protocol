using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tide.Simulator.Models;

namespace Tide.Simulator
{
    public class BlockchainContext : DbContext
    {
        public BlockchainContext(DbContextOptions<BlockchainContext> options)
            : base(options)
        {
        }

        public DbSet<BlockData> Data { get; set; }
    }
}
