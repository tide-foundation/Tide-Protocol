using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Models.Serialization
{
    public class AesKeyConverter : JsonConverter<AesKey>
    {
        public override AesKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var val = reader.GetString();
            if (string.IsNullOrEmpty(val)) return null;

            AesKey key;
            try
            {
                key = AesKey.Parse(val);
            }
            catch (ArgumentException)
            {
                throw new JsonException($"Unable to convert \"{val}\" into a AES key");
            }

            return key;
        }

        public override void Write(Utf8JsonWriter writer, AesKey value, JsonSerializerOptions options)
            => writer.WriteStringValue(Convert.ToBase64String(value.ToByteArray()));
    }
}