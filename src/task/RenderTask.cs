using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using livemap.layer.builtin;
using livemap.render;
using livemap.util;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.Common.Database;
using Vintagestory.GameContent;
using Vintagestory.Server;

namespace livemap.task;

[PublicAPI]
public sealed class RenderTask {
    // let's not create too many of these, so we don't kill the GC
    private readonly BlockPos _mutableBlockPos = new(0);

    private readonly LiveMap _server;
    private readonly RenderTaskManager _renderTaskManager;

    public RenderTask(LiveMap server, RenderTaskManager renderTaskManager) {
        _server = server;
        _renderTaskManager = renderTaskManager;
    }

    public void ScanRegion(int regionX, int regionZ) {
        try {
            ServerMapRegion? region = _renderTaskManager.ChunkLoader.GetMapRegion(ChunkPos.ToChunkIndex(regionX, 0, regionZ));
            if (region == null) {
                return;
            }

            // check for existing chunks only in this region
            int chunkX1 = regionX << 4;
            int chunkZ1 = regionZ << 4;
            int chunkX2 = chunkX1 + 16;
            int chunkZ2 = chunkZ1 + 16;

            // get blockdata from all chunks
            IEnumerable<ChunkPos> chunks = _renderTaskManager.ChunkLoader.GetAllMapChunkPositions()
                .Where(chunkPos => chunkPos.X >= chunkX1 && chunkPos.Z >= chunkZ1 && chunkPos.X < chunkX2 && chunkPos.Z < chunkZ2);
            BlockData blockData = new();
            foreach (ChunkPos chunkPos in chunks) {
                ScanChunkColumn(region, chunkPos, blockData);
            }

            // process the region through all the renderers
            foreach ((string _, Renderer renderer) in _server.RendererRegistry) {
                renderer.AllocateImage(regionX, regionZ);
                renderer.ProcessBlockData(regionX, regionZ, blockData);
                renderer.CalculateShadows();
                renderer.SaveImage();
            }
        } catch (Exception e) {
            Logger.Warn(e.ToString());
        }
    }

    private void ScanChunkColumn(ServerMapRegion region, ChunkPos chunkPos, BlockData blockData) {
        // get chunkmap from game save
        // this is just basic info about a chunk column, like heightmaps
        ServerMapChunk? mapChunk = _renderTaskManager.ChunkLoader.GetMapChunk(ChunkPos.ToChunkIndex(chunkPos.X, chunkPos.Y, chunkPos.Z));
        if (mapChunk == null) {
            return;
        }

        // check which chunk slices need to be loaded to get the top surface block
        List<int> chunkIndexesToLoad = new();
        FindChunksToLoad(region, mapChunk, chunkPos, chunkIndexesToLoad);

        // load the actual chunks slices from game save
        ServerChunk?[] chunkSlices = new ServerChunk?[_server.Sapi.WorldManager.MapSizeY >> 5];
        foreach (int y in chunkIndexesToLoad) {
            chunkSlices[y] = _renderTaskManager.ChunkLoader.GetChunk(ChunkPos.ToChunkIndex(chunkPos.X, y, chunkPos.Z));
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
        foreach ((string _, Renderer renderer) in _server.RendererRegistry) {
            renderer.ScanChunkColumn(chunkPos, blockData);
        }

        // process things from structures
        ProcessStructures(chunkPos, chunkSlices);
    }

    private void FindChunksToLoad(ServerMapRegion region, ServerMapChunk? mapChunk, ChunkPos chunkPos, List<int> chunkIndexesToLoad) {
        for (int x = 0; x < 32; x++) {
            for (int z = 0; z < 32; z++) {
                int y = GetTopBlockY(mapChunk, x, z) >> 5;
                chunkIndexesToLoad.AddIfNotExists(y);
                if (y > 0) {
                    chunkIndexesToLoad.AddIfNotExists(y - 1);
                }
            }
        }

        // check which chunk slices need to be loaded to get specific structure data
        region.GeneratedStructures?
            .Where(s =>
                s.Code.Contains("trader") ||
                s.Code.Contains("gates")
            )
            .Where(s =>
                (chunkPos.X << 5) < s.Location.MaxX &&
                (chunkPos.Z << 5) < s.Location.MaxZ &&
                (chunkPos.X << 5) + 32 > s.Location.MinX &&
                (chunkPos.Z << 5) + 32 > s.Location.MinZ
            )
            .Foreach(s => {
                for (int y = s.Location.Y1 >> 5; y <= s.Location.Y2 >> 5; y++) {
                    chunkIndexesToLoad.AddIfNotExists(y);
                }
            });
    }

    private void ProcessStructures(ChunkPos chunkPos, ServerChunk?[] chunkSlices) {
        ulong chunkIndex = chunkPos.ToChunkIndex();

        TradersLayer? tradersLayer = _server.LayerRegistry.Traders;

        HashSet<TradersLayer.Trader> traders = new();

        chunkSlices.Foreach(chunk => {
            if (_server.Config.Layers.Translocators.Enabled) {
                chunk?.BlockEntities.Values.Foreach(be => {
                    if (be is not BlockEntityStaticTranslocator { TargetLocation: not null } tl) {
                        return;
                    }

                    BlockPos pos = tl.Pos;
                    BlockPos loc = tl.TargetLocation;

                    // save tl to file
                    Logger.Warn($"Translocator at {pos} points to {loc}");
                });
            }

            if (tradersLayer != null) {
                if (_server.Config.Layers.Traders.Enabled) {
                    chunk?.Entities.Foreach(e => {
                        if (e is not EntityTrader trader) {
                            return;
                        }

                        string type = trader.GetName();
                        string? name = trader.WatchedAttributes.GetTreeAttribute("nametag")?.GetString("name");
                        BlockPos pos = trader.Pos.AsBlockPos;

                        traders.Add(new TradersLayer.Trader(type, name, pos));
                        Logger.Warn($"Trader at {pos} is named {name} (type: {type})");
                    });
                }
            }
        });

        tradersLayer?.SetTraders(chunkIndex, traders);
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
        if (!_renderTaskManager.MicroBlocks.Contains(top)) {
            return;
        }
        serverChunk.BlockEntities.TryGetValue(_mutableBlockPos.Set(x, y, z), out BlockEntity? be);
        top = be is BlockEntityMicroBlock bemb ? bemb.BlockIds[0] : _renderTaskManager.LandBlock;
    }

    private int GetTopBlockY(ServerMapChunk? mapChunk, int x, int z, int def = 0) {
        ushort? blockY = mapChunk?.RainHeightMap[Mathf.BlockIndex(x, z)];
        return GameMath.Clamp(blockY ?? def, 0, _server.Sapi.WorldManager.MapSizeY - 1);
    }
}
