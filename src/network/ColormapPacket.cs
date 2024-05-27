using ProtoBuf;

namespace livemap.network;

[ProtoContract]
public sealed class ColormapPacket : Packet {
    [ProtoMember(1)]
    public string? RawColormap;
}
