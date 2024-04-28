using livemap.common.configuration;
using ProtoBuf;

namespace livemap.common.network.packet;

[ProtoContract]
public sealed class ConfigPacket : Packet {
    [ProtoMember(1)]
    public Config? Config;
}
