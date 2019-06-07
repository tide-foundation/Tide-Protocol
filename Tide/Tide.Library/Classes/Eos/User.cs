using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Tide.Library.Classes.Eos
{
    public class User
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("account")]
        public string Account { get; set; }

        [JsonProperty("vendor")]
        public string Vendor { get; set; }

        [JsonProperty("ork_links")]
        public List<OrkLink> OrkLinks { get; set; }
    }

    public class OrkLink {
        [JsonProperty("vendor")]
        public ulong Vendor { get; set; }

        [JsonProperty("ork_ids")]
        public List<ulong> OrkIds { get; set; }
    }
}
