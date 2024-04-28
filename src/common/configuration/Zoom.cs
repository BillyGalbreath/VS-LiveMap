using ProtoBuf;

namespace livemap.common.configuration;

[ProtoContract]
public class Zoom {
    [ProtoMember(1)]
    public int Default { get; set; }

    [ProtoMember(2)]
    public int MaxIn { get; set; } = -3;

    [ProtoMember(3)]
    public int MaxOut { get; set; } = 8;
}
