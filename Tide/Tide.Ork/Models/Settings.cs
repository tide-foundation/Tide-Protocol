namespace Tide.Ork.Models {
    public class Settings
    {
        public Instance Instance { get; set; }
        public Endpoints Endpoints { get; set; }
        public EmailClient EmailClient { get; set; }
    }

    public class Instance
    {
        public string SecretKey { get; set; }
        public string Username { get; set; }
    }

    public class Endpoints
    {
        public Endpoint Simulator { get; set; }
    }

    public class Endpoint
    {
        public string Api { get; set; }
        public string Password { get; set; }
    }

    public class EmailClient
    {
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
        public string SenderHost { get; set; }
        public int SenderPort { get; set; }
    }
}