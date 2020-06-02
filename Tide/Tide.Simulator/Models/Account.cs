namespace Tide.Simulator.Models {
    public class Account {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }
    }
}