using System.Collections.Generic;
using JetBrains.Annotations;
using livemap.server;
using SkiaSharp;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace livemap.common.render;

[PublicAPI]
public class MedievalRenderer : Renderer {
    public static string GetDefaultMapColorCode(EnumBlockMaterial material) {
        return material switch {
            EnumBlockMaterial.Soil => "land",
            EnumBlockMaterial.Sand => "desert",
            EnumBlockMaterial.Ore => "land",
            EnumBlockMaterial.Gravel => "desert",
            EnumBlockMaterial.Stone => "land",
            EnumBlockMaterial.Leaves => "forest",
            EnumBlockMaterial.Plant => "plant",
            EnumBlockMaterial.Wood => "forest",
            EnumBlockMaterial.Snow => "glacier",
            EnumBlockMaterial.Liquid => "lake",
            EnumBlockMaterial.Ice => "glacier",
            EnumBlockMaterial.Lava => "lava",
            _ => "land"
        };
    }

    public static OrderedDictionary<string, string> HexColorsByCode = new() {
        { "ink", "#483018" },
        { "settlement", "#856844" },
        { "wateredge", "#483018" },
        { "land", "#AC8858" },
        { "desert", "#C4A468" },
        { "forest", "#98844C" },
        { "road", "#805030" },
        { "plant", "#808650" },
        { "lake", "#CCC890" },
        { "ocean", "#CCC890" },
        { "glacier", "#E0E0C0" }
    };

    public static readonly OrderedDictionary<string, uint> ColorsByCode = new();

    static MedievalRenderer() {
        foreach (KeyValuePair<string, string> val in HexColorsByCode) {
            ColorsByCode[val.Key] = (uint)SKColor.Parse(val.Value);
        }
    }

    private readonly byte[] _block2Color;
    private readonly bool[] _blockIsLake;

    public MedievalRenderer(LiveMapServer server) : base(server, "medieval") {
        int max = server.Api.World.Blocks.Count;
        _block2Color = new byte[max + 1];
        _blockIsLake = new bool[max + 1];

        foreach (Block block in server.Api.World.Blocks) {
            int id = block.BlockId;

            if (block.BlockMaterial == EnumBlockMaterial.Snow && block.Code.Path.Contains("snowblock")) {
                _block2Color[id] = (byte)ColorsByCode.IndexOfKey("glacier");
                _blockIsLake[id] = false;
                continue;
            }

            string colorCode = "land";
            if (block.Attributes != null) {
                colorCode = block.Attributes["mapColorCode"].AsString() ??
                            GetDefaultMapColorCode(block.BlockMaterial);
            }

            _block2Color[id] = (byte)ColorsByCode.IndexOfKey(colorCode);
            _blockIsLake[id] = IsLake(block);
        }
    }

    protected override void PostProcessRegion(int regionX, int regionZ, BlockData blockData) {
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

                uint color = GetMedievalStyleColor(id, x, z, blockData);

                float yDiff = ProcessShadow(x, y, z, blockData);

                TileImage.SetBlockColor(x, z, color, yDiff);
            }
        }
    }

    private uint GetMedievalStyleColor(int blockId, int x, int z, BlockData blockData) {
        uint pixelColor;

        if (_blockIsLake[blockId]) {
            BlockData.Data? blockIdN = blockData.Get(x, z - 1);
            BlockData.Data? blockIdE = blockData.Get(x + 1, z);
            BlockData.Data? blockIdS = blockData.Get(x, z + 1);
            BlockData.Data? blockIdW = blockData.Get(x - 1, z);

            pixelColor = (blockIdN == null || _blockIsLake[blockIdN.Top]) &&
                         (blockIdE == null || _blockIsLake[blockIdE.Top]) &&
                         (blockIdS == null || _blockIsLake[blockIdS.Top]) &&
                         (blockIdW == null || _blockIsLake[blockIdW.Top])
                ? GetColor(blockId)
                : ColorsByCode["wateredge"];
        } else {
            pixelColor = GetColor(blockId);
        }

        return pixelColor;
    }

    private uint GetColor(int block) {
        return ColorsByCode.GetValueAtIndex(_block2Color[block]);
    }


    private static bool IsLake(Block block) {
        return block.BlockMaterial == EnumBlockMaterial.Liquid || (block.BlockMaterial == EnumBlockMaterial.Ice && block.Code.Path != "glacierice");
    }
}
