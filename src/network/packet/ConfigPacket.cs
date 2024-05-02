using livemap.data;
using ProtoBuf;

namespace livemap.network.packet;

[ProtoContract]
public sealed class ConfigPacket : Packet {
    [ProtoMember(1)]
    public Config? Config;
}
