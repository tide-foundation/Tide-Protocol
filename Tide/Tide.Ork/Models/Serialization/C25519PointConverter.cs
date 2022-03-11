using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tide.Encryption.Ecc;

namespace Tide.Ork.Models.Serialization
{
    public class C25519PointConverter : JsonConverter<C25519Point>
    {
        public override C25519Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var val = reader.GetString();
            if (string.IsNullOrEmpty(val)) return C25519Point.Infinity;

            C25519Point point;
            try
            {
                point = C25519Point.From(Convert.FromBase64String(val));
            }
            catch (ArgumentException)
            {
                throw new JsonException($"Unable to convert \"{val}\" into a point");
            }

            if (!point.IsValid) {
                throw new JsonException($"Invalid point for \"{val}\"");
            }

            return point;
        }

        public override void Write(Utf8JsonWriter writer, C25519Point value, JsonSerializerOptions options)
            => writer.WriteStringValue(Convert.ToBase64String(value.ToByteArray()));
    }
}