using ProtoBuf;

namespace livemap.common.network;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public sealed class ColormapPacket : Packet {
    public string? RawColormap;
}
