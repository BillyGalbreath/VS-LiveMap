using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using livemap.common.registry;
using livemap.common.util;
using livemap.server;
using livemap.server.exception;
using livemap.server.tile;
using livemap.server.util;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.Common.Database;
using Vintagestory.GameContent;
using Vintagestory.Server;

namespace livemap.common.render;

[PublicAPI]
public abstract class Renderer : Keyed {
    public string Id { get; }
    public ICoreServerAPI Api { get; }
    public LiveMapServer Server { get; }

    protected ChunkLoader ChunkLoader { get; }
    protected TileImage? TileImage { get; set; }

    protected int LandBlock { get; set; }
    protected HashSet<int> MicroBlocks { get; set; }
    protected HashSet<int> BlocksToIgnore { get; set; }

    protected bool Cancelled { get; set; }

    protected Renderer(LiveMapServer server, ICoreServerAPI api, string id) {
        Id = id;
        Api = api;
        Server = server;

        ChunkLoader = new ChunkLoader(Api);

        LandBlock = Api.World.GetBlock(new AssetLocation("game", "soil-low-normal")).Id;

        MicroBlocks = Api.World.Blocks
            .Where(block => block.Code != null)
            .Where(block =>
                block.Code.Path.StartsWith("chiseledblock") ||
                block.Code.Path.StartsWith("microblock"))
            .Select(block => block.Id)
            .ToHashSet();

        BlocksToIgnore = Api.World.Blocks
            .Where(block => block.Code != null)
            .Where(block =>
                (block.Code.Path.EndsWith("-snow") && !MicroBlocks.Contains(block.Id)) ||
                block.Code.Path.EndsWith("-snow2") ||
                block.Code.Path.EndsWith("-snow3") ||
                block.Code.Path.Equals("snowblock") ||
                block.Code.Path.Contains("snowlayer-") ||
                block is BlockRequireSolidGround or BlockPlant)
            .Select(block => block.Id).ToHashSet();
    }

    protected void CheckIfCancelled() {
        if (Cancelled) {
            throw new RenderCancelledException();
        }
    }

    protected void AllocateImage(int regionX, int regionZ) {
        TileImage = new TileImage(regionX, regionZ, Server.Config.Zoom.MaxOut);
    }

    protected void SaveImage() {
        TileImage?.Save();
    }

    protected void CalculateShadows() {
        TileImage?.CalculateShadows();
    }

    public void ScanRegion(long region) {
        long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        int regionX = Mathf.LongToX(region);
        int regionZ = Mathf.LongToZ(region);

        try {
            ScanRegion(regionX, regionZ);
        } catch (RenderCancelledException e) {
            // render was cancelled
            // todo
            Logger.Warn(e.ToString());
        } catch (Exception e) {
            Logger.Warn(e.ToString());
            return;
        }

        long end = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Logger.Debug($"Region {regionX},{regionZ} done. ({end - start}ms)");
    }

    protected void ScanRegion(int regionX, int regionZ) {
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

        foreach (ChunkPos chunkPos in chunks) {
            ScanChunkColumn(chunkPos);
            CheckIfCancelled();
        }

        CalculateShadows();

        CheckIfCancelled();

        SaveImage();
    }

    protected void ScanChunkColumn(ChunkPos chunkPos) {
        // get chunkmap from game save
        // this is just basic info about a chunk column, like heightmaps
        ServerMapChunk? mapChunk = ChunkLoader.GetServerMapChunk(chunkPos);
        if (mapChunk == null) {
            return;
        }

        // get neighboring chunkmaps from game save
        // we'll use these to calculate heightmap on north/west chunk edge blocks
        ServerMapChunk?[] mapChunkArray = { ChunkLoader.GetServerMapChunk(chunkPos.X - 1, chunkPos.Y, chunkPos.Z - 1), ChunkLoader.GetServerMapChunk(chunkPos.X - 1, chunkPos.Y, chunkPos.Z), ChunkLoader.GetServerMapChunk(chunkPos.X, chunkPos.Y, chunkPos.Z - 1) };

        // check which chunk slices need to be loaded to get the top surface block
        List<int> chunkIndexesToLoad = new();
        for (int x = 0; x < 32; x++) {
            for (int z = 0; z < 32; z++) {
                chunkIndexesToLoad.AddIfNotExists(GetTopBlockY(mapChunk, x, z) >> 5);
            }
        }

        // load the actual chunks slices from game save
        ServerChunk?[] chunkSlices = new ServerChunk?[Api.WorldManager.MapSizeY >> 5];
        foreach (int y in chunkIndexesToLoad) {
            chunkSlices[y] = ChunkLoader.GetServerChunk(chunkPos.X, y, chunkPos.Z);
        }

        // let's not create too many of these, so we don't kill the GC
        BlockPos mutableBlockPos = new(0);

        // scan every block column in the chunk
        for (int x = 0; x < 32; x++) {
            for (int z = 0; z < 32; z++) {
                ScanBlockColumn(x, z, chunkPos, mutableBlockPos, mapChunk, chunkSlices, mapChunkArray);
            }
        }
    }

    private void ScanBlockColumn(int x, int z, ChunkPos chunkPos, BlockPos mutableBlockPos, ServerMapChunk mapChunk, IReadOnlyList<ServerChunk?> chunkSlices, IReadOnlyList<ServerMapChunk?> mapChunkArray) {
        CheckIfCancelled();

        int imgX = x + (chunkPos.X << 5);
        int imgZ = z + (chunkPos.Z << 5);

        try {
            int y = GetTopBlockY(mapChunk, x, z);
            bool ignored = false;
            uint color = ProcessColor(x, y, z, ref ignored, chunkPos, mutableBlockPos, chunkSlices);
            float yDiff = ProcessShadow(x, y, z, ignored, mapChunk, mapChunkArray);
            TileImage?.SetBlockColor(imgX, imgZ, color, yDiff);
        } catch (Exception) {
            TileImage?.SetBlockColor(imgX, imgZ, 0, 0);
        }
    }

    protected uint ProcessColor(int x, int y, int z, ref bool ignored, ChunkPos chunkPos, BlockPos mutableBlockPos, IReadOnlyList<ServerChunk?> serverChunks) {
        ServerChunk? serverChunk = serverChunks[y >> 5];
        if (serverChunk == null) {
            return 0;
        }

        int blockId = serverChunk.Data[Mathf.AsIndex(x, y, z)];

        if (BlocksToIgnore.Contains(blockId)) {
            ignored = true;
            y--;
            serverChunk = serverChunks[y >> 5] ?? ChunkLoader.GetServerChunk(chunkPos.X, y >> 5, chunkPos.Z);
            blockId = serverChunk!.Data[Mathf.AsIndex(x, y, z)];
        }

        // ReSharper disable once InvertIf
        if (MicroBlocks.Contains(blockId)) {
            mutableBlockPos.Set((chunkPos.X << 5) + x, y, (chunkPos.Z << 5) + z);
            serverChunk.BlockEntities.TryGetValue(mutableBlockPos, out BlockEntity? be);
            blockId = be is BlockEntityMicroBlock bemb ? bemb.BlockIds[0] : LandBlock;
        }

        return Server.Colormap.TryGet(blockId, out uint[]? colors) ? colors[GameMath.MurmurHash3Mod(x, y, z, colors.Length)] : 0;
    }

    private float ProcessShadow(int x, int y, int z, bool ignored, ServerMapChunk mapChunk, IReadOnlyList<ServerMapChunk?> mapChunkArray) {
        int offsetX = x - 1;
        int offsetZ = z - 1;

        ServerMapChunk? northwestChunk = offsetX switch {
            < 0 when offsetZ < 0 => mapChunkArray[0],
            < 0 => mapChunkArray[1],
            _ => offsetZ < 0 ? mapChunkArray[2] : mapChunk
        };

        if (ignored) {
            y++;
        }

        int northwest = y - GetTopBlockY(northwestChunk, offsetX, offsetZ, y);
        int west = y - GetTopBlockY(offsetX < 0 ? mapChunkArray[1] : mapChunk, offsetX, z, y);
        int north = y - GetTopBlockY(offsetZ < 0 ? mapChunkArray[2] : mapChunk, x, offsetZ, y);

        int direction = Math.Sign(northwest) + Math.Sign(north) + Math.Sign(west);
        int steepness = Math.Max(Math.Max(Math.Abs(northwest), Math.Abs(north)), Math.Abs(west));
        float slopeFactor = Math.Min(0.5F, steepness / 10F) / 1.25F;
        return direction switch {
            > 0 => 1.08F + slopeFactor,
            < 0 => 0.92F - slopeFactor,
            _ => 1
        };
    }

    protected int GetTopBlockY(ServerMapChunk? mapChunk, int x, int z, int def = 0) {
        ushort? blockY = mapChunk?.RainHeightMap[Mathf.AsIndex(x, z)];
        return GameMath.Clamp(blockY ?? def, 0, Api.WorldManager.MapSizeY - 1);
    }

    public void Dispose() {
        ChunkLoader.Dispose();
        TileImage?.Dispose();
        MicroBlocks.Clear();
        BlocksToIgnore.Clear();
    }

    public class Builder : Keyed {
        public string Id { get; }
        public System.Func<LiveMapServer, ICoreServerAPI, Renderer> Func { get; }

        public Builder(string id, System.Func<LiveMapServer, ICoreServerAPI, Renderer> func) {
            Id = id;
            Func = func;
        }
    }
}
