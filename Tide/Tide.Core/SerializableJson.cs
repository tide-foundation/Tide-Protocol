using System;
using System.Text.Json;

namespace Tide.Core
{
    public abstract class SerializableJson<TClass, TDto> where TClass : SerializableJson<TClass, TDto>, new()
    {
        protected abstract TDto GetDto();
        protected abstract void MapDto(TDto basic);

        public override string ToString() => JsonSerializer.Serialize(GetDto(), GetJsonOptions());

        public static TClass Parse(string data)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException($"An empty {nameof(data)} is not valid to deserialize a {nameof(TClass)}.");

            var obj = new TClass();
            obj.MapDto(JsonSerializer.Deserialize<TDto>(data, obj.GetJsonOptions()));
            return obj;
        }

        protected virtual JsonSerializerOptions GetJsonOptions()
        {
            return new JsonSerializerOptions
            {
                IgnoreNullValues = true, 
                PropertyNameCaseInsensitive = true
            };
        }
    }
}
