

using Microsoft.EntityFrameworkCore;
using Tide.Vendor.Controllers;

namespace Tide.Vendor.Models
{
    public class VendorDbContext : DbContext
    {
        public VendorDbContext(DbContextOptions<VendorDbContext> options)
            : base(options)
        {
        }

        public DbSet<VendorUser> Users { get; set; }
      //  public DbSet<User> Users { get; set; }
        public DbSet<RentalApplication> Applications { get; set; }
    }
}