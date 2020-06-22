using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tide.Usecase.Models
{
    public class Settings
    {
        public Endpoints Endpoints { get; set; }
    }

    public class Endpoints
    {
        public Endpoint Simulator { get; set; }
    }

    public class Endpoint
    {
        public string Api { get; set; }
        public string Password { get; set; }
    }
}
