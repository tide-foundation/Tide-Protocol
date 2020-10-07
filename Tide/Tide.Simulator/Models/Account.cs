using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Tide.Simulator.Models {
    /// <summary>
    /// This simulates an MSA/Blockchain account
    /// </summary>
    public class Account {
        [Key]
        public string OrkId { get; set; }
        public string PublicKey { get; set; }

    }
}