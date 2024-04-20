using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace livemap.Common.Api.Json;

/// <summary>
/// Converter for uint to/from string color codes
/// </summary>
public class ColorJsonConverter : JsonConverter {
    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
        if (value is uint num) {
            writer.WriteValue($"#{num:X6}");
        }
    }

    /// <inheritdoc/>
    public override object? ReadJson(JsonReader reader, Type type, object? existingValue, JsonSerializer serializer) {
        if (reader.TokenType != JsonToken.String) {
            return null;
        }

        string? hex = JToken.Load(reader).ToObject<string>()?[^6..];
        return hex != null ? uint.Parse(hex, NumberStyles.HexNumber) : null;
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type type) {
        return type.GetElementType() == typeof(string);
    }
}
