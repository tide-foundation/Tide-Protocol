namespace Tide.Ork.Models {
    public class Settings {
        public Instance Instance { get; set; }
        public Endpoints Endpoints { get; set; }
    }

    public class Instance {
        public string SecretKey { get; set; }
    }

    public class Endpoints {
        public Endpoint Simulator { get; set; }
    }

    public class Endpoint {
        public string Api { get; set; }
        public string Password { get; set; }
    }
}