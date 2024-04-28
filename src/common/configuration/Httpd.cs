using ProtoBuf;

namespace livemap.common.configuration;

[ProtoContract]
public class Httpd {
    [ProtoMember(1)]
    public bool Enabled { get; set; } = true;

    [ProtoMember(2)]
    public int Port { get; set; } = 8080;
}
