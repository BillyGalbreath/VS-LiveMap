using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace livemap.common.api.json;

/// <summary>
/// Converter for Opacity to/from double/byte
/// </summary>
public class OpacityJsonConverter : JsonConverter {
    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
        if (value is Opacity opacity) {
            writer.WriteValue(opacity.ToDouble());
        }
    }

    /// <inheritdoc/>
    public override object? ReadJson(JsonReader reader, Type type, object? existingValue, JsonSerializer serializer) {
        Console.WriteLine(reader.TokenType);

        if (reader.TokenType == JsonToken.Bytes) {
            return (Opacity)JToken.Load(reader).ToObject<byte>()!;
        }

        if (reader.TokenType != JsonToken.Float) {
            return null;
        }

        return (Opacity)JToken.Load(reader).ToObject<double>();
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type type) {
        return type.GetElementType() == typeof(string) || type.GetElementType() == typeof(uint) || type.GetElementType() == typeof(int);
    }
}
