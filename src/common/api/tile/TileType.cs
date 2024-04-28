using JetBrains.Annotations;
using ProtoBuf;
using SkiaSharp;

namespace livemap.common.api.tile;

[PublicAPI]
[ProtoContract]
public class TileType {
    public static readonly TileType Png = new("png", SKEncodedImageFormat.Png);
    public static readonly TileType Webp = new("webp", SKEncodedImageFormat.Webp);

    [ProtoMember(1)]
    public string Type { get; }

    [ProtoMember(2)]
    public SKEncodedImageFormat Format { get; }

    private TileType(string type, SKEncodedImageFormat format) {
        Type = type;
        Format = format;
    }

    public override string ToString() {
        return Type;
    }
}
