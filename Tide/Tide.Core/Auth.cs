using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tide.Core {
    public class Auth
    {
        public int Id { get; set; }

        public Guid TranId { get; set; }
        
        public Guid Uid { get; set; }

        [Column(TypeName = "VARCHAR(MAX)")]
        public string SuccessfulOrks { get; set; }

        [Column(TypeName = "VARCHAR(MAX)")]
        public string UnsuccessfulOrks { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string Method { get; set; }

        public bool Successful { get; set; }

        public DateTimeOffset Time { get; set; }
    }
}