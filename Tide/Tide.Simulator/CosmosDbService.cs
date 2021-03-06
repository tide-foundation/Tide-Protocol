﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Tide.Core;
using Tide.Simulator.Classes;
using Tide.Simulator.Models;

namespace Tide.Simulator {
    public class CosmosDbService : IBlockLayer {
        private readonly Container _transactionContainer;
 
        private const string TRANSACTION_CONTAINER = "Transactions";
        private const string ACCOUNT_CONTAINER = "Accounts";
       
        public CosmosDbService(Settings settings) {
        
            var db = settings.CosmosDbSettings.Database;
        
            var client = new CosmosClientBuilder(settings.CosmosDbSettings.Connection)
                .WithSerializerOptions(new CosmosSerializationOptions {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                })
                .Build();

            var database = client.CreateDatabaseIfNotExistsAsync(db).Result;

            database.Database.CreateContainerIfNotExistsAsync(TRANSACTION_CONTAINER, "/location").Wait();
            _transactionContainer = client.GetContainer(db, TRANSACTION_CONTAINER);

        }

        public (bool success, string error) Write(Transaction block) {
            return Write(new List<Transaction>() {block});
        }

        public (bool success, string error) Write(List<Transaction> blocks)
        {
            try {
                var batch = _transactionContainer.CreateTransactionalBatch(new PartitionKey(blocks.First().Location));
                foreach (var transaction in blocks)
                {
                    batch = CreateStaleBatch(batch, transaction.Location, transaction.Index);
                    batch.CreateItem(transaction);
                }

                if (batch.ExecuteAsync().Result.IsSuccessStatusCode) return (true, null);
                return (false, batch.ExecuteAsync().Result.ErrorMessage);

            }
            catch (Exception e) {
                return (false, e.Message);
         
            }

        }

        public List<Transaction> Read(string contract, string table, string scope, KeyValuePair<string,string> index)
        {
            var queryDefinition = new QueryDefinition($"select * from c where c.location = '{Transaction.CreateLocation(contract,table,scope)}' AND c.data['{index.Key}'] = '{index.Value}'");
            var query = _transactionContainer.GetItemQueryIterator<Transaction>(queryDefinition);
            var results = new List<Transaction>();
            while (query.HasMoreResults) results.AddRange(query.ReadNextAsync().Result);
            return results.ToList();
        }

        public Transaction Read(string location, string index)
        {
            return Fetch(location, index);
        }

        public List<Transaction> Read(string location, ICollection<string> index) {
            return _transactionContainer.GetItemLinqQueryable<Transaction>(true).Where(t => t.Location == location && !t.Stale && index.Contains(t.Index)).ToList();
        }

        public Transaction Read(string contract, string table, string scope, string index) {
            return Fetch(contract,table,scope,index);
        }

        public List<Transaction> Read(string contract, string table, string scope) {
            return _transactionContainer.GetItemLinqQueryable<Transaction>(true).Where(t => t.Location == Transaction.CreateLocation(contract, table, scope) && !t.Stale).ToList();
        }

        public bool SetStale(string contract, string table, string scope, string index) {
            var res = CreateStaleBatch(null, Transaction.CreateLocation(contract, table, scope), index).ExecuteAsync().Result;
            return res.IsSuccessStatusCode;
        }

        public List<Transaction> ReadHistoric(string contract, string table, string scope, string index)
        {
            return _transactionContainer.GetItemLinqQueryable<Transaction>(true).Where(t => t.Location == Transaction.CreateLocation(contract, table, scope) && t.Index == index).ToList();
        }

        public List<Transaction> ReadHistoric() {
            return _transactionContainer.GetItemLinqQueryable<Transaction>(true).ToList();
        }

        #region Helpers

        private Transaction Fetch(string contract, string table, string scope, string index) {
            return Fetch(Transaction.CreateLocation(contract, table, scope), index);
        }

        private Transaction Fetch(string location, string index) {
            return _transactionContainer.GetItemLinqQueryable<Transaction>(true).Where(t => t.Location == location && t.Index == index && !t.Stale).AsEnumerable().FirstOrDefault();
        }


        private TransactionalBatch CreateStaleBatch(TransactionalBatch batch, string location, string index) {
            if(batch == null) batch = _transactionContainer.CreateTransactionalBatch(new PartitionKey(location));
            var transaction = Fetch(location, index);
            if (transaction == null) return batch;
            transaction.Stale = true;

            return null;
           // return batch.ReplaceItem(transaction.Id, transaction);
        }

        public List<AuthStateTran> SelectPendingLogs(IEnumerable<Guid> ids, int threshold)
        {
            throw new NotImplementedException();
        }

        public List<Auth> SelectLogs(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public void UpdateLogs(IEnumerable<Auth> logs)
        {
            throw new NotImplementedException();
        }

        public void InsertLogs(IEnumerable<Auth> logs)
        {
            throw new NotImplementedException();
        }

        public void InsertPendingLogs(IEnumerable<AuthPending> logs)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}