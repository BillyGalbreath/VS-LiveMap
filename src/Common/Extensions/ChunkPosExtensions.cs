using Vintagestory.Common.Database;

namespace LiveMap.Common.Extensions;

public static class ChunkPosExtensions {
    public static ChunkPos Set(this ref ChunkPos chunkPos, int x, int y, int z) {
        chunkPos.X = x;
        chunkPos.Y = y;
        chunkPos.Z = z;
        return chunkPos;
    }
}
