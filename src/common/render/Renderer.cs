using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using livemap.common.exception;
using livemap.common.registry;
using livemap.common.tile;
using livemap.common.util;
using livemap.server;
using livemap.server.util;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.Common.Database;
using Vintagestory.GameContent;
using Vintagestory.Server;

namespace livemap.common.render;

[PublicAPI]
public abstract class Renderer : Keyed {
    // let's not create too many of these, so we don't kill the GC
    protected readonly BlockPos _mutableBlockPos = new(0);

    public string Id { get; }
    public LiveMapServer Server { get; }

    protected ChunkLoader ChunkLoader { get; }
    protected TileImage? TileImage { get; set; }

    protected HashSet<int> MicroBlocks { get; }
    protected HashSet<int> BlocksToIgnore { get; }
    protected int LandBlock { get; set; }

    protected bool Cancelled { get; set; }

    protected Renderer(LiveMapServer server, string id) {
        Id = id;
        Server = server;

        ChunkLoader = new ChunkLoader(Server.Api);

        MicroBlocks = Server.Api.World.Blocks
            .Where(block => block.Code != null)
            .Where(block =>
                block.Code.Path.StartsWith("chiseledblock") ||
                block.Code.Path.StartsWith("microblock"))
            .Select(block => block.Id)
            .ToHashSet();

        BlocksToIgnore = Server.Api.World.Blocks
            .Where(block => block.Code != null)
            .Where(block =>
                (block.Code.Path.EndsWith("-snow") && !MicroBlocks.Contains(block.Id)) ||
                block.Code.Path.EndsWith("-snow2") ||
                block.Code.Path.EndsWith("-snow3") ||
                block.Code.Path.Equals("snowblock") ||
                block.Code.Path.Contains("snowlayer-") ||
                block is BlockRequireSolidGround)
            .Select(block => block.Id).ToHashSet();

        LandBlock = Server.Api.World.GetBlock(new AssetLocation("game", "soil-low-normal")).Id;
    }

    protected virtual void CheckIfCancelled() {
        if (Cancelled) {
            throw new RenderCancelledException();
        }
    }

    protected virtual void AllocateImage(int regionX, int regionZ) {
        TileImage = new TileImage(Server, regionX, regionZ);
    }

    protected virtual void SaveImage() {
        TileImage?.Save(Id);
    }

    protected virtual void CalculateShadows() {
        TileImage?.CalculateShadows();
    }

    public virtual void ScanRegion(long region) {
        long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        int regionX = Mathf.LongToX(region);
        int regionZ = Mathf.LongToZ(region);

        try {
            ScanRegion(regionX, regionZ);
        } catch (RenderCancelledException) {
            // render was cancelled
        } catch (Exception e) {
            Logger.Warn(e.ToString());
            return;
        }

        long end = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Logger.Debug($"Region {regionX},{regionZ} finished ({end - start}ms)");
    }

    protected virtual void ScanRegion(int regionX, int regionZ) {
        // check for existing chunks only in this region
        int x1 = regionX << 4;
        int z1 = regionZ << 4;
        int x2 = x1 + 16;
        int z2 = z1 + 16;
        IEnumerable<ChunkPos> chunks = ChunkLoader.GetAllMapChunkPositions()
            .Where(pos => pos.X >= x1 && pos.Z >= z1 && pos.X < x2 && pos.Z < z2);

        CheckIfCancelled();

        AllocateImage(regionX, regionZ);

        CheckIfCancelled();

        BlockData blockData = new();
        foreach (ChunkPos chunkPos in chunks) {
            ScanChunkColumn(chunkPos, blockData);
            CheckIfCancelled();
        }

        PostProcessRegion(regionX, regionZ, blockData);

        CheckIfCancelled();

        CalculateShadows();

        CheckIfCancelled();

        SaveImage();
    }

    protected virtual void ScanChunkColumn(ChunkPos chunkPos, BlockData blockData) {
        // get chunkmap from game save
        // this is just basic info about a chunk column, like heightmaps
        ServerMapChunk? mapChunk = ChunkLoader.GetServerMapChunk(chunkPos);
        if (mapChunk == null) {
            return;
        }

        int startX = chunkPos.X << 5;
        int startZ = chunkPos.Z << 5;
        int endX = startX + 32;
        int endZ = startZ + 32;

        // check which chunk slices need to be loaded to get the top surface block
        List<int> chunkIndexesToLoad = new();
        for (int x = startX; x < endX; x++) {
            for (int z = startZ; z < endZ; z++) {
                chunkIndexesToLoad.AddIfNotExists(GetTopBlockY(mapChunk, x, z) >> 5);
            }
        }

        // load the actual chunks slices from game save
        ServerChunk?[] chunkSlices = new ServerChunk?[Server.Api.WorldManager.MapSizeY >> 5];
        foreach (int y in chunkIndexesToLoad) {
            chunkSlices[y] = ChunkLoader.GetServerChunk(chunkPos.X, y, chunkPos.Z);
        }

        // scan every block column in the chunk
        for (int x = startX; x < endX; x++) {
            for (int z = startZ; z < endZ; z++) {
                blockData.Set(x & 511, z & 511, ScanBlockColumn(x & 31, z & 31, mapChunk, chunkSlices));
            }
        }
    }

    protected virtual BlockData.Data ScanBlockColumn(int x, int z, ServerMapChunk? mapChunk, ServerChunk?[] chunkSlices) {
        CheckIfCancelled();

        int y = 0;
        int top = 0;
        int under = 0;

        try {
            y = GetTopBlockY(mapChunk, x, z);
            ServerChunk? serverChunk = chunkSlices[y >> 5];
            if (serverChunk != null) {
                top = serverChunk.Data[Mathf.BlockIndex(x, y, z)];
                CheckForMicroBlocks(x, y, z, serverChunk, ref top);

                under = serverChunk.Data[Mathf.BlockIndex(x, y - 1, z)];
                CheckForMicroBlocks(x, y - 1, z, serverChunk, ref under);
            }
        } catch (Exception) {
            // ignore
        }

        return new BlockData.Data(y, top, under);
    }

    protected virtual void PostProcessRegion(int regionX, int regionZ, BlockData blockData) { }

    protected virtual (int, int) ProcessBlock(BlockData.Data? block, int defY = 0) {
        if (block == null) {
            return (0, defY);
        }
        int id, y;
        if (BlocksToIgnore.Contains(block.Top)) {
            id = block.Under;
            y = block.Y - 1;
        } else {
            id = block.Top;
            y = block.Y;
        }
        return (id, y);
    }

    protected virtual float ProcessShadow(int x, int y, int z, BlockData blockData) {
        (int _, int northwest) = ProcessBlock(blockData.Get(x - 1, z - 1), y);
        (int _, int north) = ProcessBlock(blockData.Get(x, z - 1), y);
        (int _, int west) = ProcessBlock(blockData.Get(x - 1, z), y);

        int direction = Math.Sign(y - northwest) + Math.Sign(y - north) + Math.Sign(y - west);
        int steepness = Math.Max(Math.Max(Math.Abs(y - northwest), Math.Abs(y - north)), Math.Abs(y - west));
        float slopeFactor = Math.Min(0.5F, steepness / 10F) / 1.25F;
        return direction switch {
            > 0 => 1.08F + slopeFactor,
            < 0 => 0.92F - slopeFactor,
            _ => 1
        };
    }

    protected virtual void CheckForMicroBlocks(int x, int y, int z, ServerChunk serverChunk, ref int top) {
        if (!MicroBlocks.Contains(top)) {
            return;
        }
        serverChunk.BlockEntities.TryGetValue(_mutableBlockPos.Set(x, y, z), out BlockEntity? be);
        top = be is BlockEntityMicroBlock bemb ? bemb.BlockIds[0] : LandBlock;
    }

    protected virtual int GetTopBlockY(ServerMapChunk? mapChunk, int x, int z, int def = 0) {
        ushort? blockY = mapChunk?.RainHeightMap[Mathf.BlockIndex(x, z)];
        return GameMath.Clamp(blockY ?? def, 0, Server.Api.WorldManager.MapSizeY - 1);
    }

    public virtual void Dispose() {
        ChunkLoader.Dispose();
        TileImage?.Dispose();
        MicroBlocks.Clear();
        BlocksToIgnore.Clear();
    }

    public class Builder : Keyed {
        public string Id { get; }
        public System.Func<LiveMapServer, Renderer> Func { get; }

        public Builder(string id, System.Func<LiveMapServer, Renderer> func) {
            Id = id;
            Func = func;
        }
    }
}
