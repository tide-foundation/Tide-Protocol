namespace Tide.Simulator.Models {
    /// <summary>
    /// This simulates an MSA/Blockchain account
    /// </summary>
    public class Account {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string Hash { get; set; }
        public string Salt { get; set; }

        public string Data { get; set; }
    }
}