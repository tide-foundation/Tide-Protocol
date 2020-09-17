using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tide.Core;
using Tide.Vendor.Models;
using User = Tide.Vendor.Models.User;

namespace Tide.Vendor.Controllers
{
    public class BaseController : ControllerBase
    {
        protected VendorDbContext Context;
        public BaseController(VendorDbContext context)
        {
            Context = context;
        }
        private User _user;
        protected new User User
        {
            get { return _user ??= Context.Users.Include(u=>u.RentalApplications).FirstOrDefault(u => u.Id == HttpContext.User.Identity.Name); }
            set => _user = value;
        }
    }
}
