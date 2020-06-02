using System;

namespace Tide.Simulator.Models {
    public class BlockData {
        // Misc
        public int Id { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public bool Stale { get; set; }

        // Location
        public Contract Contract { get; set; }
        public Table Table { get; set; }
        public string Scope { get; set; }
        public string Index { get; set; }

        // Payload
        public string Data { get; set; }
    }
}