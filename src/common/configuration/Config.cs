using ProtoBuf;

namespace livemap.common.configuration;

[ProtoContract]
public class Config {
    [ProtoMember(1)]
    public Httpd Httpd { get; set; } = new();

    [ProtoMember(2)]
    public Web Web { get; set; } = new();

    [ProtoMember(3)]
    public Zoom Zoom { get; set; } = new();
}
