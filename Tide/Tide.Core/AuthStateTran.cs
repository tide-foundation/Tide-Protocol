using System;

namespace Tide.Core {
    public class AuthStateTran
    {
        public Guid TranId  { get; set; }
        public bool Successful  { get; set; }
        public int Count  { get; set; }
        public string Orks  { get; set; }
    }
}