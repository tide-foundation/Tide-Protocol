using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tide.BlockchainSimulator.Models;
using Tide.Library.Models;

namespace Tide.BlockchainSimulator
{
    public class BlockchainContext : DbContext
    {
        public BlockchainContext(DbContextOptions<BlockchainContext> options)
            : base(options)
        {
        }

        public DbSet<JsonData> Data { get; set; }
    }
}