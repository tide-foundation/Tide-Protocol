using Microsoft.EntityFrameworkCore;
using Tide.Simulator.Models;

namespace Tide.Simulator {
    public class BlockchainContext : DbContext {
        public BlockchainContext(DbContextOptions<BlockchainContext> options)
            : base(options) {
        }


        public DbSet<Account> Account { get; set; }
        public DbSet<BlockData> Data { get; set; }
    }
}