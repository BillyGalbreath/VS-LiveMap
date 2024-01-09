using ProtoBuf;

namespace LiveMap.Common.Network;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class ColormapPacket : Packet {
    public byte[]? RawColormap;
}
