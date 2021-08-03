using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Tide.Core;

namespace Tide.Simulator
{
    public class BlockchainContext : DbContext
    {
        public BlockchainContext(DbContextOptions<BlockchainContext> options) : base(options) {
            
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<AuthPending> AuthPendings { get; set; }
        public DbSet<Auth> Auths { get; set; }
    }
}
