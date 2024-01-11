using System.Collections.Generic;
using ProtoBuf;
using Vintagestory.API.Util;

namespace LiveMap.Common.Util;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public sealed class Colormap {
    private readonly Dictionary<string, int[]> colors = new();

    public void Add(string block, int[] toAdd) {
        colors.Add(block, toAdd);
    }

    public int[]? Get(string block) {
        return colors!.Get(block);
    }

    public override string ToString() {
        return "Colormap[Colors=" + colors + "]";
    }

    public byte[] Serialize() {
        return SerializerUtil.Serialize(this);
    }

    public static Colormap Deserialize(byte[] data) {
        return SerializerUtil.Deserialize<Colormap>(data);
    }

    public void Dispose() {
        colors.Clear();
    }
}
