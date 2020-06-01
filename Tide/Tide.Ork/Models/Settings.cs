
namespace Tide.Ork.Models {
    public class Settings {
        public Instance Instance { get; set; }
        public Endpoints Endpoints { get; set; }
    }

    public class Instance {
        public string SecretKey { get; set; }
    }

    public class Endpoints {
        public string Simulator { get; set; }
    }
}