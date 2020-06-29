using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tide.Usecase.Models;

namespace Tide.Usecase
{
    public class VendorContext : DbContext
    {
        public VendorContext(DbContextOptions<VendorContext> options)
            : base(options)
        {
        }


        public DbSet<VendorUser> Users { get; set; }
        public DbSet<Application> Applications { get; set; }
    }
}