using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using livemap.common.extensions;
using livemap.common.util;
using livemap.server.util;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.Common.Database;
using Vintagestory.GameContent;
using Vintagestory.Server;

namespace livemap.server.render;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public abstract class Renderer {
    protected readonly RenderTask _renderTask;
    protected readonly DataLoader _dataLoader;

    protected readonly int _landBlock;
    protected readonly HashSet<int> _microBlocks;
    protected readonly HashSet<int> _blocksToIgnore;

    protected TileImage? _image;

    protected ICoreServerAPI Api => _renderTask.Server.Api;

    protected Renderer(RenderTask renderTask) {
        _renderTask = renderTask;
        _dataLoader = new DataLoader(Api);

        _landBlock = Api.World.GetBlock(new AssetLocation("game", "soil-low-normal")).Id;

        _microBlocks = Api.World.Blocks
            .Where(block => block.Code != null)
            .Where(block =>
                block.Code.Path.StartsWith("chiseledblock") ||
                block.Code.Path.StartsWith("microblock"))
            .Select(block => block.Id)
            .ToHashSet();

        _blocksToIgnore = Api.World.Blocks
            .Where(block => block.Code != null)
            .Where(block =>
                    (block.Code.Path.EndsWith("-snow") && !_microBlocks.Contains(block.Id))
                    || block.Code.Path.EndsWith("-snow2")
                    || block.Code.Path.EndsWith("-snow3")
                    || block.Code.Path.Equals("snowblock")
                    || block.Code.Path.Contains("snowlayer-")
                /*|| block is BlockRequireSolidGround or BlockPlant*/)
            .Select(block => block.Id).ToHashSet();
    }

    public void ScanAllRegions() {
        long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Logger.Debug("&3Begin scanning all existing regions");

        // sort out the chunks per region
        HashSet<long> regions = new();
        ImmutableList<ChunkPos> allChunks = _dataLoader.GetAllMapChunkPositions().ToImmutableList();
        foreach (ChunkPos chunk in allChunks) {
            regions.Add(Mathf.AsLong(chunk.X >> 4, chunk.Z >> 4));

            if (_renderTask.Stopped) {
                return;
            }
        }

        // inform what we found
        Logger.Debug($"&3Started scanning {allChunks.Count} chunks in {regions.Count} regions");

        // scan one region at a time
        foreach (long region in regions) {
            ScanRegion(region, allChunks);

            if (_renderTask.Stopped) {
                return;
            }
        }

        long end = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Logger.Debug($"&3Finished scanning {allChunks.Count} chunks in {regions.Count} regions ({end - start}ms)");
    }

    public void ScanRegion(long region, IEnumerable<ChunkPos>? allChunks = null) {
        long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        int regionX = Mathf.LongToX(region);
        int regionZ = Mathf.LongToZ(region);

        // check for existing chunks only in this region
        int x1 = regionX << 4;
        int z1 = regionZ << 4;
        int x2 = x1 + 16;
        int z2 = z1 + 16;
        IEnumerable<ChunkPos> regionChunks = (allChunks ?? _dataLoader.GetAllMapChunkPositions())
            .Where(pos => pos.X >= x1 && pos.Z >= z1 && pos.X < x2 && pos.Z < z2);

        if (_renderTask.Stopped) {
            return;
        }

        AllocateImage(regionX, regionZ);

        if (_renderTask.Stopped) {
            return;
        }

        ScanRegion(regionChunks);

        if (_renderTask.Stopped) {
            return;
        }

        CalculateShadows();

        if (_renderTask.Stopped) {
            return;
        }

        Save();

        if (_renderTask.Stopped) {
            return;
        }

        long end = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Logger.Debug($"Region {regionX},{regionZ} done. ({end - start}ms)");
    }

    protected void AllocateImage(int regionX, int regionZ) {
        _image = new TileImage(regionX, regionZ);
    }

    protected void CalculateShadows() {
        _image?.CalculateShadows();
    }

    protected void Save() {
        _image?.Save();
    }

    protected void ScanRegion(IEnumerable<ChunkPos> chunks) {
        foreach (ChunkPos chunkPos in chunks) {
            ScanChunk(chunkPos);
        }
    }

    protected void ScanChunk(ChunkPos chunkPos) {
        ServerMapChunk? mapChunk = _dataLoader.GetServerMapChunk(chunkPos);
        if (mapChunk == null) {
            return;
        }

        ServerMapChunk?[] mapChunkArray = {
            _dataLoader.GetServerMapChunk(chunkPos.X - 1, chunkPos.Y, chunkPos.Z - 1),
            _dataLoader.GetServerMapChunk(chunkPos.X - 1, chunkPos.Y, chunkPos.Z),
            _dataLoader.GetServerMapChunk(chunkPos.X, chunkPos.Y, chunkPos.Z - 1)
        };

        // check which chunk need to be loaded to get the top surface block
        List<int> chunksToLoad = new();
        for (int x = 0; x < 32; x++) {
            for (int z = 0; z < 32; z++) {
                chunksToLoad.AddIfNotExists(GetTopBlockY(mapChunk, x, z) >> 5);
            }
        }

        // load chunks from database
        ServerChunk?[] serverChunks = new ServerChunk?[Api.WorldManager.MapSizeY >> 5];
        foreach (int y in chunksToLoad) {
            serverChunks[y] = _dataLoader.GetServerChunk(chunkPos.X, y, chunkPos.Z);
        }

        BlockPos tmpPos = new(0);
        for (int x = 0; x < 32; x++) {
            for (int z = 0; z < 32; z++) {
                ScanBlock(x, z, chunkPos, tmpPos, mapChunk, serverChunks, mapChunkArray);
            }
        }
    }

    private void ScanBlock(int x, int z, ChunkPos chunkPos, BlockPos tmpPos, ServerMapChunk mapChunk, IReadOnlyList<ServerChunk?> serverChunks, IReadOnlyList<ServerMapChunk?> mapChunkArray) {
        if (_renderTask.Stopped) {
            return;
        }

        int imgX = x + (chunkPos.X << 5);
        int imgZ = z + (chunkPos.Z << 5);

        try {
            int blockY = GetTopBlockY(mapChunk, x, z);
            bool ignored = false;
            uint color = ProcessColor(x, blockY, z, ref ignored, chunkPos, tmpPos, serverChunks);
            float yDiff = ProcessShadow(x, blockY, z, ignored, mapChunk, mapChunkArray);
            _image?.SetBlockColor(imgX, imgZ, color, yDiff);
        } catch (Exception) {
            _image?.SetBlockColor(imgX, imgZ, 0, 0);
        }
    }

    private uint ProcessColor(int x, int y, int z, ref bool ignored, ChunkPos chunkPos, BlockPos tmpPos, IReadOnlyList<ServerChunk?> serverChunks) {
        ServerChunk? serverChunk = serverChunks[y >> 5];
        if (serverChunk == null) {
            return 0;
        }

        int blockId = serverChunk.Data[Mathf.AsIndex(x, y, z)];

        if (_blocksToIgnore.Contains(blockId)) {
            ignored = true;
            y--;
            serverChunk = serverChunks[y >> 5] ?? _dataLoader.GetServerChunk(chunkPos.X, y >> 5, chunkPos.Z);
            blockId = serverChunk!.Data[Mathf.AsIndex(x, y, z)];
        }

        // ReSharper disable once InvertIf
        if (_microBlocks.Contains(blockId)) {
            tmpPos.Set((chunkPos.X << 5) + x, y, (chunkPos.Z << 5) + z);
            serverChunk.BlockEntities.TryGetValue(tmpPos, out BlockEntity? be);
            blockId = be is BlockEntityMicroBlock bemb ? bemb.BlockIds[0] : _landBlock;
        }

        return _renderTask.Server.Colormap.TryGet(blockId, out uint[]? colors) ? colors[GameMath.MurmurHash3Mod(x, y, z, colors.Length)] : 0;
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
        return GameMath.Clamp(mapChunk?.RainHeightMap[Mathf.AsIndex(x, z)] ?? def, 0, Api.WorldManager.MapSizeY - 1);
    }
}
