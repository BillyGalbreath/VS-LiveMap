using JetBrains.Annotations;
using livemap.server;
using Vintagestory.API.MathTools;

namespace livemap.common.render;

[PublicAPI]
public class BasicRenderer : Renderer {
    public BasicRenderer(LiveMapServer server) : base(server, "basic") { }

    public override void PostProcessRegion(int regionX, int regionZ, BlockData blockData) {
        if (TileImage == null) {
            return;
        }

        for (int x = 0; x < 512; x++) {
            for (int z = 0; z < 512; z++) {
                BlockData.Data? block = blockData.Get(x, z);
                if (block == null) {
                    continue;
                }

                (int id, int y) = ProcessBlock(block);

                uint color = 0;
                if (Server.Colormap.TryGet(id, out uint[]? colors)) {
                    color = colors[GameMath.MurmurHash3Mod(x, y, z, colors.Length)];
                }

                float yDiff = ProcessShadow(x, y, z, blockData);

                TileImage.SetBlockColor(x, z, color, yDiff);
            }
        }
    }
}
