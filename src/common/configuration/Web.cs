using livemap.common.tile;
using ProtoBuf;

namespace livemap.common.configuration;

[ProtoContract]
public class Web {
    [ProtoMember(1)]
    public string Path { get; set; } = "web/";

    [ProtoMember(2)]
    public bool ReadOnly { get; set; }

    [ProtoMember(3)]
    public TileType TileType { get; set; } = TileType.Webp;

    [ProtoMember(4)]
    public int TileQuality { get; set; } = 100;
}
