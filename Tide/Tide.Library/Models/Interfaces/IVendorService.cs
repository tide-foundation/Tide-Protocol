namespace Tide.Library.Models.Interfaces {
    public interface IVendorService {

        /// <summary>
        ///     Creates a top-tier Tide account which serves as a vendor. New users can be brought on board below this account at
        ///     the vendors expense
        /// </summary>
        /// <param name="payer">Blockchain account funding the creation of the account</param>
        /// <param name="account">Blockchain account to be associated to the account</param>
        /// <param name="username">Tide username to be associated to the account</param>
        /// <param name="publicKey">ElGamal public key used for transit encryption</param>
        /// <param name="description">A short description of the vendor and it's purpose</param>
        /// <returns></returns>
        TideResponse CreateVendor(string payer, string account, string username, string publicKey, string description);
    }
}