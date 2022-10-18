using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tide.Encryption.Ed;
using System.Numerics;

namespace Tide.Ork.Models.Serialization
{
    public class Ed25519PointConverter : JsonConverter<Ed25519Point>
    {
        public override Ed25519Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var val = reader.GetString();
            if (string.IsNullOrEmpty(val)) return new Ed25519Point(BigInteger.Zero, BigInteger.One, BigInteger.One, BigInteger.Zero); //infinity also known as identity for ed25519

            Ed25519Point point;
            try
            {
                point = Ed25519Point.From(Convert.FromBase64String(val));
            }
            catch (ArgumentException)
            {
                throw new JsonException($"Unable to convert \"{val}\" into a point");
            }

            if (!point.IsValid()) {
                throw new JsonException($"Invalid point for \"{val}\"");
            }

            return point;
        }

        public override void Write(Utf8JsonWriter writer, Ed25519Point value, JsonSerializerOptions options)
            => writer.WriteStringValue(Convert.ToBase64String(value.ToByteArray()));
    }
}