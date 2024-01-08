using System.Collections.Generic;
using ProtoBuf;
using Vintagestory.API.Util;

namespace LiveMap.Common.Util;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class BlockColors {
    public Dictionary<string, int[]> Colors { get; } = new();

    public override string ToString() {
        return "BlockColors[Colors=" + Colors + "]";
    }

    public byte[] Serialize() {
        return SerializerUtil.Serialize(this);
    }

    public static BlockColors Deserialize(byte[] data) {
        return SerializerUtil.Deserialize<BlockColors>(data);
    }
}
