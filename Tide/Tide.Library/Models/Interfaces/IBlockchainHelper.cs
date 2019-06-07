namespace Tide.Library.Models.Interfaces {
    public interface IBlockchainHelper {
        TideResponse InitializeAccount(string username);
        TideResponse ConfirmAccount(string username);
        TideResponse CreateVendor(CreateVendorModel model);

        // Ork functions
        TideResponse GetNodes(string username);
        TideResponse PostFragment(AuthenticationModel model);
        TideResponse GetFragment(AuthenticationModel model);
    }
}