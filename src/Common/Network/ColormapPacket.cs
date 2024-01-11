using ProtoBuf;

namespace LiveMap.Common.Network;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public sealed class ColormapPacket : Packet {
    public byte[]? RawColormap;
}
