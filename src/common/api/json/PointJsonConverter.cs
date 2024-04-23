using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace livemap.common.api.json;

/// <summary>
/// Converter for array to/from point
/// </summary>
public class PointJsonConverter : JsonConverter {
    /// <inheritdoc/>
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
        if (value is not Point point) {
            return;
        }
        writer.WriteStartArray();
        point = point.Floor();
        writer.WriteValue((int)point.X);
        writer.WriteValue((int)point.Z);
        writer.WriteEndArray();
    }

    /// <inheritdoc/>
    public override object? ReadJson(JsonReader reader, Type type, object? existingValue, JsonSerializer serializer) {
        if (reader.TokenType == JsonToken.StartObject) {
            return JToken.Load(reader).ToObject<Point>();
        }

        if (reader.TokenType != JsonToken.StartArray) {
            return null;
        }

        return (Point)JToken.Load(reader).ToObject<double[]>()!;
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type type) {
        return type.IsArray && type.GetElementType() == typeof(double);
    }
}
