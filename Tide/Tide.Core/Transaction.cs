using System;
using Newtonsoft.Json;

namespace Tide.Core {
    public class Transaction
    {

        //public Transaction()
        //{

        //}

        public Transaction(string contract, string table, string scope, string index, object data)
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTimeOffset.Now;
            Index = index;
            Data = data;

            Location = CreateLocation(contract, table, scope);
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

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
        public object Data { get; set; }



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
            return $"{contract}/{table}/{scope}";
        }
    }


}