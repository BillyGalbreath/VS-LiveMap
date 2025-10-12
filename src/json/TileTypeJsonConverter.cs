using livemap.tile;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace livemap.json;

public class TileTypeJsonConverter : JsonConverter {
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
        if (value is TileType) {
            writer.WriteValue(value.ToString());
        }
    }

    public override object? ReadJson(JsonReader reader, Type type, object? existingValue, JsonSerializer serializer) {
        if (reader.TokenType != JsonToken.String) {
            return null;
        }

        string? str = JToken.Load(reader).ToObject<string>();
        return str == null ? null : TileType.Types.TryGetValue(str);
    }

    public override bool CanConvert(Type type) {
        return type.GetElementType() == typeof(string);
    }
}
