using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Tide.Encryption.AesMAC;

namespace Tide.VendorSdk.Classes
{
    public class TideVendorRepo : IVendorRepo
    {
        private readonly TideVendor _client;

        public TideVendorRepo(TideVendor client)
        {
            _client = client;
        }

        //TODO: How do I confirm the orks
        public Task ConfirmUser(Guid vuid)
        {
            var res = _client.ConfirmUser(ToString(vuid));
            return res.Success ? Task.CompletedTask
                : Task.FromException(new Exception(res.Error));
        }

        //TODO: The key is missing
        public Task CreateUser(Guid vuid, AesKey auth, List<string> orks)
        {
            var res = _client.CreateUser(ToString(vuid), orks);
            return res.Success ? Task.CompletedTask
                : Task.FromException(new Exception(res.Error));
        }

        public Task<AesKey> GetKey(Guid vuid)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetListOrks()
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetListOrks(Guid vuid)
        {
            var res = _client.GetUserNodes(ToString(vuid));
            if (!res.Success)
                return Task.FromException<List<string>>(new Exception(res.Error));

            return Task.FromResult(Deserialize<List<OrkConf>>(res.Content).Select(itm => itm.Ork).ToList());
        }

        //TODO: This is an unknown error 
        public Task RollbackUser(Guid vuid)
        {
            var res = _client.RollbackUser(ToString(vuid));
            return res.Success ? Task.CompletedTask
                : Task.FromException(new Exception(res.Error));
        }

        private static string ToString(Guid data) => Convert.ToBase64String(data.ToByteArray());

        private static T Deserialize<T>(object data)
        {
            return JsonSerializer.Deserialize<T>(data.ToString(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        private class OrkConf
        {
            public string Ork { get; set; }
            public bool Confirmed { get; set; }
        }
    }
}