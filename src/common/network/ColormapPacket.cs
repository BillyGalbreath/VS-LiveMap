using ProtoBuf;

namespace livemap.common.network;

[ProtoContract]
public sealed class ColormapPacket : Packet {
    [ProtoMember(1)]
    public string? RawColormap;
}
