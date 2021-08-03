using System;
using System.Linq;
using Newtonsoft.Json;

namespace Tide.Core {
    public class Transaction
    {

        public Transaction()
        {

        }

        public Transaction(string contract, string table, string scope, string index,string account, object data)
        {
            DateCreated = DateTimeOffset.Now;
            Index = index;
            Account = account;
            Data = data is string ? data as string : JsonConvert.SerializeObject(data);

            Location = CreateLocation(contract, table, scope);
        }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "account")]
        public string Account { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }

        [JsonProperty(PropertyName = "index")]
        public string Index { get; set; }

        [JsonProperty(PropertyName = "created")]
        public DateTimeOffset DateCreated { get; set; }

        [JsonProperty(PropertyName = "isStale")]
        public bool Stale { get; set; }

        // Payload
        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }

        [JsonProperty(PropertyName = "sign")]
        public byte[] Sign { get; set; }



        public T GetData<T>() {
            try {
                return JsonConvert.DeserializeObject<T>(Data.ToString());
            }
            catch (Exception) {
                return default;
            }
        }

        public static string CreateLocation(string contract, string table, string scope)
        {
            return string.Join('/', (new[] { contract, table, scope }).Where(elm => !String.IsNullOrWhiteSpace(elm)));
        }
    }
}