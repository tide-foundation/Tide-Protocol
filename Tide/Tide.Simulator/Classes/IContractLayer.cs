using Tide.Simulator.Models;

namespace Tide.Simulator.Classes {
    public interface IContractLayer {
        /// <summary>
        ///     Fetch a user from the 'Blockchain'
        /// </summary>
        /// <param name="username">The username of the user you wish the fetch.</param>
        /// <returns>The user account. Otherwise null.</returns>
        SimulatorUser GetUser(string username);

        /// <summary>
        ///     Create a new user on the 'Blockchain'.
        /// </summary>
        /// <param name="username">The username of the user. This will be used as the primary index.</param>
        /// <param name="password">TEMP: This will need to be replaced with some pub/priv protection</param>
        /// <returns>The user account. Otherwise null.</returns>
        SimulatorUser CreateUser(string username);

        /// <summary>
        /// Adds a fragment for the user into the 'Blockchain'.
        /// </summary>
        /// <param name="username">The username fo the user. This will be used as the scope.</param>
        /// <param name="ork">The ork username. This will be the primary key.</param>
        /// <returns>True if the write was successful.</returns>
        //  bool AddFrag(string username, string ork, Keyvault);
    }
}