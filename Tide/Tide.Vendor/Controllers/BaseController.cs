using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tide.Core;
using Tide.Vendor.Models;

namespace Tide.Vendor.Controllers
{
    public class BaseController : ControllerBase
    {
        protected VendorDbContext Context;
        public BaseController(VendorDbContext context)
        {
            Context = context;
        }
        private ApplicationUser _user;
        protected new ApplicationUser User
        {
            get { return _user ??= Context.Users.FirstOrDefault(u => u.Id == HttpContext.User.Identity.Name); }
            set => _user = value;
        }
    }
}
