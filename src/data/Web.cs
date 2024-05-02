using JetBrains.Annotations;
using livemap.json;
using livemap.tile;
using Newtonsoft.Json;

namespace livemap.data;

[PublicAPI]
public class Web {
    public string Path { get; set; } = "web/";

    public bool ReadOnly { get; set; }

    [JsonConverter(typeof(TileTypeJsonConverter))]
    public TileType TileType { get; set; } = TileType.Webp;

    public int TileQuality { get; set; } = 100;
}
