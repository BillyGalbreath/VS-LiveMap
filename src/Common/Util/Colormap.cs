using System.Collections.Generic;
using ProtoBuf;
using Vintagestory.API.Util;

namespace LiveMap.Common.Util;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public sealed class Colormap {
    private readonly Dictionary<string, int[]> _colors = new();

    public void Add(string block, int[] toAdd) {
        _colors.Add(block, toAdd);
    }

    public int[]? Get(string block) {
        return _colors!.Get(block);
    }

    public override string ToString() {
        return "Colormap[Colors=" + _colors + "]";
    }

    public byte[] Serialize() {
        return SerializerUtil.Serialize(this);
    }

    public static Colormap Deserialize(byte[] data) {
        return SerializerUtil.Deserialize<Colormap>(data);
    }

    public void Dispose() {
        _colors.Clear();
    }
}
