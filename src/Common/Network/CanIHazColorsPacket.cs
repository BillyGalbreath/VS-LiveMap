using ProtoBuf;

namespace LiveMap.Common.Network;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class CanIHazColorsPacket : Packet {
    public byte[] Colors;
}
