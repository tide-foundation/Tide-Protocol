namespace Tide.Simulator.Models {
    public class Settings {
        public string Connection { get; set; }
        public string BearerKey { get; set; }
        public CosmosDbSettings CosmosDbSettings { get; set; }
    }

    public class CosmosDbSettings {
        public string Connection { get; set; }
        public string Database { get; set; }
        public string Container { get; set; }
    }
}