using Tide.VendorSdk.Classes.Storage;

namespace Tide.VendorSdk.Configuration
{
    public interface ITideConfiguration
    {

    }

    public class TideConfiguration : ITideConfiguration {
        public static TideConfiguration Configuration = new TideConfiguration();
        public IDatabase Database { get; set; }
        public string VendorId { get; set; }
        public string Endpoint { get; set; }
    }
}
