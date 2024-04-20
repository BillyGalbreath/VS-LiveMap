using System;
using LiveMap.Common.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace livemap.Common.Api.Json;

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
        writer.WriteValue(point.X);
        writer.WriteValue(point.Z);
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

        double[]? arr = JToken.Load(reader).ToObject<double[]>();
        return arr != null ? new Point(arr[0], arr[1]) : null;
    }

    /// <inheritdoc/>
    public override bool CanConvert(Type type) {
        return type.IsArray && type.GetElementType() == typeof(double);
    }
}
