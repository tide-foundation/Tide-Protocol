﻿using System;
using System.Collections.Generic;
using Tide.Core;

namespace Tide.Simulator.Classes {
    public interface IBlockLayer
    {
        /// <summary>
        ///     Write to the 'blockchain'. If there is data in the designated location, it will mark the current data stale and
        ///     insert the updated data.
        /// </summary>
        /// <param name="contract">The contract you want to write to.</param>
        /// <param name="table">The table you want to write to.</param>
        /// <param name="scope">
        ///     Which scope (username) you want to write to. Each user can have entirely different data sets under
        ///     their own scopes.
        /// </param>
        /// <param name="index">The primary index for the data under the selected scope.</param>
        /// <param name="data">The payload you wish to save to the blockchain.</param>
        /// <returns>True if the write was successful</returns>
        (bool success,string error) Write(Transaction block);
        (bool success, string error) Write(List<Transaction> blocks);

        List<AuthStateTran> SelectPendingLogs(IEnumerable<Guid> ids, int threshold);
        List<Auth> SelectLogs(IEnumerable<Guid> ids);
        void UpdateLogs(IEnumerable<Auth> logs);
        void InsertLogs(IEnumerable<Auth> logs);
        void InsertPendingLogs(IEnumerable<AuthPending> logs);

        /// <summary>
        ///     Read the newest version of this data block from the 'Blockchain'.
        /// </summary>
        /// <typeparam name="T">The Type you want to deserialize to.</typeparam>
        /// <param name="location">The location of the resource.</param>
        /// <param name="index">The primary index for the data under the selected scope.</param>
        /// <returns>Serialized object</returns>
        Transaction Read(string location, string index);

        /// <summary>
        /// Read the newest versions of the data blocks from the 'Blockchain'.
        /// </summary>
        /// <param name="index">The primary index for the data under the selected scope.</param>
        /// <param name="locations">The location resources.</param>
        /// <returns>Serialized object</returns>
        List<Transaction> Read(string locations, ICollection<string> index);

        /// <summary>
        ///     Read the newest version of this data block from the 'Blockchain'.
        /// </summary>
        /// <typeparam name="T">The Type you want to deserialize to.</typeparam>
        /// <param name="contract">The contract you want to read from.</param>
        /// <param name="table">The table you want to read from.</param>
        /// <param name="scope">
        ///     Which scope (username) you want to read from. Each user can have entirely different data sets under
        ///     their own scopes.
        /// </param>
        /// <param name="index">The primary index for the data under the selected scope.</param>
        /// <returns>Serialized object</returns>
        Transaction Read(string contract, string table, string scope, string index);



        /// <summary>
        ///     Read all the non-stale data in a scope
        /// </summary>
        /// <typeparam name="T">The Type you want to deserialize to.</typeparam>
        /// <param name="contract">The contract you want to read from.</param>
        /// <param name="table">The table you want to read from.</param>
        /// <param name="scope">The scope you want to read from.</param>
        /// <returns>A list of serialized objects</returns>
        List<Transaction> Read(string contract, string table, string scope);

        /// <summary>
        ///     Read all the non-stale data in a scope
        /// </summary>
        /// <typeparam name="T">The Type you want to deserialize to.</typeparam>
        /// <param name="contract">The contract you want to read from.</param>
        /// <param name="table">The table you want to read from.</param>
        /// <param name="scope">The scope you want to read from.</param>
        /// <param name="index">Column and value of the object(s) you wish to return.</param>
        /// <returns>A list of serialized objects</returns>
        List<Transaction> Read(string contract, string table, string scope, KeyValuePair<string, string> index);


        /// <summary>
        ///     This effectively deletes an index from a smart contract
        /// </summary>
        /// <param name="contract">The contract you want to delete from.</param>
        /// <param name="table">The contract you want to delete from.</param>
        /// <param name="scope">The contract you want to delete from.</param>
        /// <param name="index">The index you want to delete from.</param>
        /// <returns></returns>
        bool SetStale(string contract, string table, string scope, string index);

        /// <summary>
        ///     The same as 'Read', but will pull the full historical data of the item.
        /// </summary>
        List<Transaction> ReadHistoric(string contract, string table, string scope, string index);

        /// <summary>
        ///     The same as 'Read', but will pull the full historical data of the item.
        /// </summary>
        List<Transaction> ReadHistoric();
    }
}