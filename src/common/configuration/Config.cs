using livemap.common.api.tile;
using ProtoBuf;

namespace livemap.common.configuration;

[ProtoContract]
public class Config {
    [ProtoMember(1)]
    public Httpd Httpd { get; set; } = new();

    [ProtoMember(2)]
    public Web Web { get; set; } = new();

    [ProtoMember(3)]
    public Zoom Zoom { get; set; } = new();
}

[ProtoContract]
public class Httpd {
    [ProtoMember(1)]
    public bool Enabled { get; set; } = true;

    [ProtoMember(2)]
    public int Port { get; set; } = 8080;
}

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

[ProtoContract]
public class Zoom {
    [ProtoMember(1)]
    public int Default { get; set; }

    [ProtoMember(2)]
    public int MaxIn { get; set; } = -3;

    [ProtoMember(3)]
    public int MaxOut { get; set; } = 8;
}
