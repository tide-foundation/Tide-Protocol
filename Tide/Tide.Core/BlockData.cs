using System;

namespace Tide.Core {
    public class BlockData
    {

        public BlockData()
        {

        }

        public BlockData(string contract, string table, string scope, string index, string data)
        {
            Contract = contract;
            Table = table;
            Scope = scope;
            Index = index;
            Data = data;
        }

        // Misc
        public int Id { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public bool Stale { get; set; }

        // Location
        public string Contract { get; set; }
        public string Table { get; set; }
        public string Scope { get; set; }
        public string Index { get; set; }

        // Payload
        public string Data { get; set; }
    }

}