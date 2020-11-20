using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Tide.Core;
using Tide.Ork.Classes;

namespace Tide.Ork.Repo {
    public class SimulatorOrkManager {
        private readonly string _orkId;
        private readonly SimulatorClient _client;

        private string TableName => "Ork";
        private string Contract => "Registration";
        
        public SimulatorOrkManager(string orkId, SimulatorClient client)
        {
            _orkId = orkId;
            _client = client;
        }

        public Task<OrkNode> Get() => GetById(_orkId);

        public async Task<OrkNode> GetById(string orkId)
        {
            var response = await _client.Get(Contract, TableName, TableName, orkId);
            if (string.IsNullOrEmpty(response))
                return default(OrkNode);

            return Map(response);
        }

        public async Task<bool> Exist() => await GetById(_orkId) == null;

        public async Task<bool> Exist(string id) => await GetById(id) == null;

        public async Task<List<OrkNode>> GetAll()
        {
            var response = await _client.Get(Contract, TableName, TableName);
            return response.Select(Map).ToList();
        }

        public async Task<TideResponse> Add(OrkNode entity)
        {
            if (await Exist(entity.Id))
                return new TideResponse($"The entity [{entity.Id}] already exists");

            return await SetOrUpdate(entity);
        }

        public async Task<TideResponse> SetOrUpdate(OrkNode entity)
        {
            var result = await _client.Post(Contract, TableName, TableName, _orkId, Map(entity));
            if (result.success) return new TideResponse(true);
            return new TideResponse(false, null, result.error);
        }

        private OrkNode Map(string data) => JsonSerializer.Deserialize<OrkNode>(data);

        private string Map(OrkNode entity) => JsonSerializer.Serialize(entity);
    }
}