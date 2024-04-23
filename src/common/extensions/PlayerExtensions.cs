using livemap.common.api;
using Vintagestory.API.Common.Entities;

namespace livemap.common.extensions;

public static class PlayerExtensions {
    public static Point ToPoint(this EntityPos pos) {
        return new Point(pos.X, pos.Z);
    }
}
