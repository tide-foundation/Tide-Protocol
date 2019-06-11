namespace Tide.Library.Models.Interfaces {
    public interface ITideProtocol {

        // Misc
        bool AccountExists(string username);

        /// <summary>
        /// Converts a plain text username into a Tide-friendly ulong
        /// </summary>
        /// <param name="data">The username you wish to convert</param>
        /// <returns></returns>
        ulong HashUsername(string data);

        // Master

        /// <summary>
        ///     Creates a top-level vendor account in which a business can run and have user-accounts signed up below it.
        /// </summary>
        /// <param name="blockchainAccount">Blockchain account for the new vendor</param>
        /// <param name="username">Tide Username for the new vendor</param>
        /// <param name="publicKey">Elgamal public key the vendor will use for encryption</param>
        /// <param name="description">A small synopsys of the vendor</param>
        /// <returns>Content: Blockchain transaction ID</returns>
        TideResponse CreateVendor(string blockchainAccount, string username, string publicKey, string description);

        // Vendor

        /// <summary>
        ///     Initializes an account to be created. This is step one and can not be skipped.
        /// </summary>
        /// <param name="publicKey">The public key used for the blockchain account</param>
        /// <param name="username">The username for the new Tide account</param>
        /// <returns>Content: Blockchain transaction ID</returns>
        TideResponse InitializeAccount(string publicKey, string username);

        /// <summary>
        ///     Finalizes the account once the client has confirmed all fragments have successfully been stored by the Orks.
        ///     Alternatives if the user decided not to use Orks and has taken note of the keys.
        /// </summary>
        /// <param name="username">The Tide username to confirm</param>
        /// <returns>Content: Blockchain transaction ID</returns>
        TideResponse ConfirmAccount(string username);

        // Ork functions

        /// <summary>
        ///     Gathers the ork nodes which the Tide user used to distribute their key
        /// </summary>
        /// <param name="username">Username of the Tide account</param>
        /// <returns>Content: Array of ork nodes</returns>
        TideResponse GetNodes(string username);
        TideResponse PostFragment(AuthenticationModel model);
        TideResponse GetFragment(AuthenticationModel model);
    }
}