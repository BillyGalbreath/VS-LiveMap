using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using livemap.data;
using livemap.logger;
using livemap.render;
using livemap.util;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.Common.Database;
using Vintagestory.GameContent;
using Vintagestory.Server;

namespace livemap.task;

[PublicAPI]
public sealed class RenderTask {
    // let's not create too many of these, so we don't kill the GC
    private readonly BlockPos _mutableBlockPos = new(0);

    private readonly LiveMapServer _server;
    private readonly ChunkLoader _chunkLoader;

    public HashSet<int> MicroBlocks { get; }
    public HashSet<int> BlocksToIgnore { get; }

    public Dictionary<string, Renderer> Renderers { get; } = new();

    private readonly int _landBlock;


    public RenderTask(LiveMapServer server) {
        _server = server;

        _chunkLoader = new ChunkLoader(_server.Api);

        MicroBlocks = _server.Api.World.Blocks
            .Where(block => block.Code != null)
            .Where(block =>
                block.Code.Path.StartsWith("chiseledblock") ||
                block.Code.Path.StartsWith("microblock"))
            .Select(block => block.Id)
            .ToHashSet();

        BlocksToIgnore = _server.Api.World.Blocks
            .Where(block => block.Code != null)
            .Where(block =>
                (block.Code.Path.EndsWith("-snow") && !MicroBlocks.Contains(block.Id)) ||
                block.Code.Path.EndsWith("-snow2") ||
                block.Code.Path.EndsWith("-snow3") ||
                block.Code.Path.Equals("snowblock") ||
                block.Code.Path.Contains("snowlayer-") ||
                block is BlockRequireSolidGround)
            .Select(block => block.Id).ToHashSet();

        _landBlock = _server.Api.World.GetBlock(new AssetLocation("game", "soil-low-normal")).Id;
    }

    public void ScanRegion(int regionX, int regionZ) {
        try {
            // check for existing chunks only in this region
            int x1 = regionX << 4;
            int z1 = regionZ << 4;
            int x2 = x1 + 16;
            int z2 = z1 + 16;
            IEnumerable<ChunkPos> chunks = _chunkLoader.GetAllMapChunkPositions()
                .Where(pos => pos.X >= x1 && pos.Z >= z1 && pos.X < x2 && pos.Z < z2);
            BlockData blockData = new();
            foreach (ChunkPos chunkPos in chunks) {
                ScanChunkColumn(chunkPos, blockData);
            }

            // process the region through all the renderers
            foreach ((string? _, Renderer? renderer) in Renderers) {
                renderer.AllocateImage(regionX, regionZ);
                renderer.ProcessBlockData(regionX, regionZ, blockData);
                renderer.CalculateShadows();
                renderer.SaveImage();
            }
        } catch (Exception e) {
            Logger.Warn(e.ToString());
        }
    }

    private void ScanChunkColumn(ChunkPos chunkPos, BlockData blockData) {
        // get chunkmap from game save
        // this is just basic info about a chunk column, like heightmaps
        ServerMapChunk? mapChunk = _chunkLoader.GetServerMapChunk(chunkPos);
        if (mapChunk == null) {
            return;
        }

        // check which chunk slices need to be loaded to get the top surface block
        List<int> chunkIndexesToLoad = new();
        for (int x = 0; x < 32; x++) {
            for (int z = 0; z < 32; z++) {
                int y = GetTopBlockY(mapChunk, x, z) >> 5;
                chunkIndexesToLoad.AddIfNotExists(y);
                if (y > 0) {
                    chunkIndexesToLoad.AddIfNotExists(y - 1);
                }
            }
        }

        // load the actual chunks slices from game save
        ServerChunk?[] chunkSlices = new ServerChunk?[_server.Api.WorldManager.MapSizeY >> 5];
        foreach (int y in chunkIndexesToLoad) {
            chunkSlices[y] = _chunkLoader.GetServerChunk(chunkPos.X, y, chunkPos.Z);
        }

        int startX = chunkPos.X << 5;
        int startZ = chunkPos.Z << 5;
        int endX = startX + 32;
        int endZ = startZ + 32;

        // scan every block column in the chunk
        for (int x = startX; x < endX; x++) {
            for (int z = startZ; z < endZ; z++) {
                blockData.Set(x & 511, z & 511, ScanBlockColumn(x & 31, z & 31, mapChunk, chunkSlices));
            }
        }

        // process the chunk through all the renderers
        foreach ((string? _, Renderer? renderer) in Renderers) {
            renderer.ScanChunkColumn(chunkPos, blockData);
        }
    }

    private BlockData.Data ScanBlockColumn(int x, int z, ServerMapChunk? mapChunk, ServerChunk?[] chunkSlices) {
        int y = 0;
        int top = 0;
        int under = 0;

        try {
            y = GetTopBlockY(mapChunk, x, z);
            ServerChunk? serverChunk = chunkSlices[y >> 5];
            if (serverChunk != null) {
                top = serverChunk.Data[Mathf.BlockIndex(x, y, z)];
                CheckForMicroBlocks(x, y, z, serverChunk, ref top);
            }
            serverChunk = chunkSlices[(y - 1) >> 5];
            if (serverChunk != null) {
                under = serverChunk.Data[Mathf.BlockIndex(x, y - 1, z)];
                CheckForMicroBlocks(x, y - 1, z, serverChunk, ref under);
            }
        } catch (Exception) {
            // ignore
        }

        return new BlockData.Data(y, top, under);
    }

    private void CheckForMicroBlocks(int x, int y, int z, ServerChunk serverChunk, ref int top) {
        if (!MicroBlocks.Contains(top)) {
            return;
        }
        serverChunk.BlockEntities.TryGetValue(_mutableBlockPos.Set(x, y, z), out BlockEntity? be);
        top = be is BlockEntityMicroBlock bemb ? bemb.BlockIds[0] : _landBlock;
    }

    private int GetTopBlockY(ServerMapChunk? mapChunk, int x, int z, int def = 0) {
        ushort? blockY = mapChunk?.RainHeightMap[Mathf.BlockIndex(x, z)];
        return GameMath.Clamp(blockY ?? def, 0, _server.Api.WorldManager.MapSizeY - 1);
    }

    public void Dispose() {
        _chunkLoader.Dispose();
        MicroBlocks.Clear();
        BlocksToIgnore.Clear();

        foreach ((string? _, Renderer renderer) in Renderers) {
            renderer.Dispose();
        }
        Renderers.Clear();
    }
}
