using ProtoBuf;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace LiveMap.Common.Util;

public class Colors {
    public static byte[] SerializeColors(ICoreClientAPI api) {
        Data data = new();
        BlockPos pos = api.World.Player.Entity.SidedPos.AsBlockPos;

        foreach (Block block in api.World.Blocks.Where(block => block.Code != null)) {
            data.Colors.Add(block.Code.ToString(), block.GetColorWithoutTint(api, pos));
        }

        return SerializerUtil.Serialize(data);
    }

    public static Dictionary<string, int> DeserializeColors(byte[] data) {
        return SerializerUtil.Deserialize<Data>(data).Colors;
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    private class Data {
        internal Dictionary<string, int> Colors = new();
    }
}
