using JetBrains.Annotations;
using SkiaSharp;
using Vintagestory.API.Datastructures;

namespace livemap.tile;

[PublicAPI]
public class TileType {
    public static readonly OrderedDictionary<string, TileType> Types = new();

    public static readonly TileType Png = Register(new TileType("png", SKEncodedImageFormat.Png));
    public static readonly TileType Webp = Register(new TileType("webp", SKEncodedImageFormat.Webp));

    private static TileType Register(TileType tileType) {
        Types.Add(tileType.Type, tileType);
        return tileType;
    }

    public string Type { get; }

    public SKEncodedImageFormat Format { get; }

    private TileType(string type, SKEncodedImageFormat format) {
        Type = type;
        Format = format;
    }

    public override string ToString() {
        return Type;
    }
}
