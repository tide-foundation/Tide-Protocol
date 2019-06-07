namespace Tide.Library.Models.Interfaces {
    public interface IBlockchainHelper {

        // Misc
        bool AccountExists(string username);

        // Master
        TideResponse CreateBlockchainAccount(string publicKey);
        TideResponse CreateVendor(CreateVendorModel model);

        // Vendor
        TideResponse InitializeAccount(string username);
        TideResponse ConfirmAccount(string username);

        // Ork functions
        TideResponse GetNodes(string username);
        TideResponse PostFragment(AuthenticationModel model);
        TideResponse GetFragment(AuthenticationModel model);
    }
}