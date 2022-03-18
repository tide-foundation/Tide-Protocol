using System;
using System.Linq;

namespace Tide.Simulator.Models {
    public class Settings {
        public string Connection { get; set; }
        public string BearerKey { get; set; }
        public string AccountConnection { get; set; }
        public int Threshold { get; set; }

        public CosmosDbSettings CosmosDbSettings { get; set; }
        public Features Features { get; set; }

        public Settings() {
            Threshold = 20;
            Features = new Features();
        }
    }

    public class Features {
        public bool DisableSignatures { get; set; }

        internal string Display() => string.Join(Environment.NewLine,
            typeof(Features).GetProperties().Select(prop => $"{prop.Name}: {prop.GetValue(this, null)}"));
    }

    public class CosmosDbSettings {
        public string Connection { get; set; }
        public string Database { get; set; }
    }
}