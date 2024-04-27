using livemap.common.api;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace livemap.common.extensions;

public static class PlayerExtensions {
    public static Point GetPoint(this IPlayer player) {
        return player.Entity.GetPoint();
    }

    public static Point GetPoint(this EntityPlayer player) {
        EntityPos pos = player.SidedPos;
        return new Point(pos.X, pos.Z);
    }
}
