using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tide.Usecase.Models
{
    public class VendorUser
    {
        public int Id { get; set; }
        public string TideId { get; set; }
        public string Token { get; set; }
        public string Public { get; set; }
    }
}
