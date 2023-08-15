using ProtoBuf;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace LiveMap.Common.Util;

public class Colors {
    private readonly Dictionary<string, int> colors;

    private Colors(Data data) {
        colors = data.Colors;
    }

    public override string ToString() {
        return "Colors[" + string.Join(",", colors.Select(e => e.Key + "=" + e.Value).ToArray()) + "]";
    }

    public static byte[] Serialize(ICoreClientAPI api) {
        Data data = new();
        BlockPos pos = api.World.Player.Entity.SidedPos.AsBlockPos;
        foreach (var block in api.World.Blocks.Where(block => block.Code != null)) {
            data.Colors.Add(block.Code.ToString(), block.GetColorWithoutTint(api, pos));
        }
        return SerializerUtil.Serialize(data);
    }

    public static Colors Deserialize(byte[] data) {
        return new Colors(SerializerUtil.Deserialize<Data>(data));
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    private class Data {
        internal Dictionary<string, int> Colors = new();
    }
}
