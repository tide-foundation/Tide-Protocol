using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tide.BlockchainSimulator.Models
{
    public class JsonData
    {
        public int Id { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public string UserId { get; set; }
        public bool Stale { get; set; }

        public Contract Contract { get; set; }
        public Table Table { get; set; }
        public string Index { get; set; }

        public string Data { get; set; }
    }

}
