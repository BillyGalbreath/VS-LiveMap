using ProtoBuf;

namespace LiveMap.Common.Network;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class BlockColorsPacket : Packet {
    public byte[]? RawDataColors;
}
