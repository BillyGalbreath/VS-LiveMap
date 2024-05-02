using JetBrains.Annotations;

namespace livemap.render;

[PublicAPI]
public class MedievalRenderer : Renderer {
    protected bool IsWater(int? id) => id == null || Server.SepiaColors.BlockIsWater[(int)id];
    protected byte GetIndex(int id) => Server.SepiaColors.Block2Color[id];
    protected uint GetColor(int id) => Server.SepiaColors.ColorsByCode.GetValueAtIndex(id);
    protected uint GetColor(string id) => Server.SepiaColors.ColorsByCode[id];

    public MedievalRenderer(LiveMapServer server) : base(server, "medieval") { }

    public override void ProcessBlockData(int regionX, int regionZ, BlockData blockData) {
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

                uint color = IsWater(id)
                    ? IsWater(blockData.Get(x, z - 1)?.Top) &&
                      IsWater(blockData.Get(x + 1, z)?.Top) &&
                      IsWater(blockData.Get(x, z + 1)?.Top) &&
                      IsWater(blockData.Get(x - 1, z)?.Top)
                        ? GetColor(GetIndex(id))
                        : GetColor("wateredge")
                    : GetColor(GetIndex(id));

                float yDiff = ProcessShadow(x, y, z, blockData);

                TileImage.SetBlockColor(x, z, color, yDiff);
            }
        }
    }
}
