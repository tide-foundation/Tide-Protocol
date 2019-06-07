namespace Tide.Library.Models.Interfaces {
    public interface IVendorAuthentication {
        /// <summary>
        /// Sets in motion the creation of an account. A vendor is required to initialize an account before fragments can be distributed
        /// </summary>
        /// <param name="username">The Tide username of the new account to be initialized</param>
        /// <returns></returns>
        TideResponse InitializeAccount(string username);

        /// <summary>
        /// Finalize an account. This is called after the end user confirmed to the vendor that all fragments have been stored successfully.
        /// An account is only usable after it has been confirmed by the vendor which initialized it.
        /// </summary>
        /// <param name="username">The Tide username to be confirmed</param>
        /// <returns></returns>
        TideResponse ConfirmAccount(string username);
    }
}