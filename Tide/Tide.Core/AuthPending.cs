using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tide.Core {
    public class AuthPending
    {
        public int Id { get; set; }

        public Guid TranId { get; set; }
        
        public Guid Uid { get; set; }

        [Column(TypeName = "VARCHAR(400)")]
        public string OrkId { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Method { get; set; }

        public bool Successful { get; set; }

        public DateTimeOffset Time { get; set; }

        [Column(TypeName = "VARCHAR(MAX)")]
        public string Metadata { get; set; }

        public AuthPending() => Time = DateTimeOffset.UtcNow;

        public override string ToString() => $"{TranId}|{Uid}|{OrkId}|{Method}|{Successful}|{Time.ToUnixTimeSeconds()}|{Metadata}";

        public  static AuthPending Parse(string data) {
            var props = data.Split('|');
            return new AuthPending() {
                TranId = Guid.Parse(props[0]),
                Uid = Guid.Parse(props[1]),
                OrkId = props[2],
                Method = props[3],
                Successful = bool.Parse(props[4]),
                Time = DateTimeOffset.FromUnixTimeSeconds(long.Parse(props[5])),
                Metadata = props[6]
            };
        }
    }
}