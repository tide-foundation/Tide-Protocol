namespace Tide.Library.Models.Interfaces {
    public interface ITideProtocol {

        // Misc
        bool AccountExists(string username);

        // Master
        TideResponse CreateBlockchainAccount(string publicKey);
        TideResponse CreateVendor(CreateVendorModel model);

        // Vendor
        TideResponse InitializeAccount(string userAccount, string username);
        TideResponse ConfirmAccount(string vendorUsername, string username);

        // Ork functions
        TideResponse GetNodes(string username);
        TideResponse PostFragment(AuthenticationModel model);
        TideResponse GetFragment(AuthenticationModel model);
    }
}