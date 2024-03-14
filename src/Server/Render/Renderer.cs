using System;
using System.Collections.Generic;
using System.Linq;
using LiveMap.Common.Extensions;
using LiveMap.Common.Util;
using LiveMap.Server.Util;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.Common.Database;
using Vintagestory.GameContent;
using Vintagestory.Server;

namespace LiveMap.Server.Render;

public abstract class Renderer {
    private readonly RenderTask _renderTask;
    private readonly DataLoader _dataLoader;

    private readonly HashSet<int> _microBlocks;
    private readonly HashSet<int> _blocksToIgnore;

    private Colormap? Colormap => _renderTask.Server.Colormap;
    private ICoreServerAPI Api => _renderTask.Server.Api;

    protected Renderer(RenderTask renderTask) {
        _renderTask = renderTask;
        _dataLoader = new DataLoader(Api);

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
        IEnumerable<ChunkPos> allChunks = _dataLoader.GetAllMapChunkPositions();
        HashSet<long> regionChunks = new();
        foreach (ChunkPos chunk in allChunks) {
            regionChunks.Add(Mathf.AsLong(chunk.X >> 4, chunk.Z >> 4));
        }

        foreach (long region in regionChunks) {
            ScanRegion(region);
        }
    }

    public void ScanRegion(long region) {
        ScanRegion(region, _dataLoader.GetAllMapChunkPositions());
    }

    public void ScanRegion(long region, IEnumerable<ChunkPos> allChunks) {
        int regionX = Mathf.LongToX(region);
        int regionZ = Mathf.LongToZ(region);

        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Logger.Debug($"##### 1) [{now}] &3Scanning Region {regionX},{regionZ}");
        // check for existing chunks only in this region
        int x1 = regionX << 4;
        int z1 = regionZ << 4;
        int x2 = x1 + 16;
        int z2 = z1 + 16;
        IEnumerable<ChunkPos> regionChunks = allChunks.Where(pos => pos.X >= x1 && pos.Z >= z1 && pos.X < x2 && pos.Z < z2);

        // allocate image
        TileImage image = new(regionX, regionZ);

        // scan the region
        ScanRegion(image, regionChunks);

        // calculate shadows
        image.CalculateShadows();

        // save image
        image.Save();

        long then = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        Logger.Debug($"##### 2) [{then} ({then - now}ms)]");
    }

    private void ScanRegion(TileImage image, IEnumerable<ChunkPos> regionChunks) {
        // tmp shared variables
        ChunkPos tmpPos = new();

        foreach (ChunkPos chunkPos in regionChunks) {
            ServerMapChunk? mapChunk = _dataLoader.GetServerMapChunk(chunkPos);
            if (mapChunk == null) {
                continue;
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
                serverChunks[y] = _dataLoader.GetServerChunk(tmpPos.Set(chunkPos.X, y, chunkPos.Z));
                serverChunks[y]?.Unpack_ReadOnly();
            }

            // get topmost block
            for (int x = 0; x < 32; x++) {
                for (int z = 0; z < 32; z++) {
                    int imgX = x + (chunkPos.X << 5);
                    int imgZ = z + (chunkPos.Z << 5);

                    try {
                        int blockY = GetTopBlockY(mapChunk, x, z);
                        int chunkY = blockY >> 5;
                        int isOff = 0;

                        ServerChunk? serverChunk = serverChunks[chunkY];
                        if (serverChunk == null) {
                            continue;
                        }

                        int blockId = serverChunk.Data[GetChunkIndex(x, blockY, z)];

                        if (_blocksToIgnore.Contains(blockId)) {
                            isOff = 1;
                            blockY--;
                            chunkY = blockY >> 5;
                            serverChunk = serverChunks[chunkY];
                            if (serverChunk is null) {
                                serverChunk = serverChunks[chunkY] = _dataLoader.GetServerChunk(tmpPos.Set(chunkPos.X, chunkY, chunkPos.Z));
                                serverChunk?.Unpack_ReadOnly();
                            }

                            blockId = serverChunk!.Data[GetChunkIndex(x, blockY, z)];
                        }

                        string blockCode;
                        if (_microBlocks.Contains(blockId)) {
                            BlockPos blockPos = new((chunkPos.X << 5) + x, blockY, (chunkPos.Z << 5) + z, 0);
                            serverChunk.BlockEntities.TryGetValue(blockPos, out BlockEntity? blockEntity);

                            if (blockEntity is BlockEntityMicroBlock blockEntityMicroBlock) {
                                blockCode = Api.World.Blocks[blockEntityMicroBlock.BlockIds[0]].Code.ToString();
                            } else {
                                // default to land for invalid chiselblock
                                blockCode = "game:soil-low-normal";
                            }
                        } else {
                            blockCode = Api.World.GetBlock(blockId).Code.ToString();
                        }

                        int color = GetBlockColor(blockCode, x, blockY, z);

                        int offsetX = x - 1;
                        int offsetZ = z - 1;

                        ServerMapChunk? northwestChunk = offsetX switch {
                            < 0 when offsetZ < 0 => mapChunkArray[0],
                            < 0 => mapChunkArray[1],
                            _ => offsetZ < 0 ? mapChunkArray[2] : mapChunk
                        };

                        blockY += isOff;

                        int northwest = blockY - (northwestChunk?.RainHeightMap[GetChunkIndex(offsetX, offsetZ)] ?? blockY);
                        int west = blockY - ((offsetX < 0 ? mapChunkArray[1] : mapChunk)?.RainHeightMap[GetChunkIndex(offsetX, z)] ?? blockY);
                        int north = blockY - ((offsetZ < 0 ? mapChunkArray[2] : mapChunk)?.RainHeightMap[GetChunkIndex(x, offsetZ)] ?? blockY);

                        int direction = Math.Sign(northwest) + Math.Sign(north) + Math.Sign(west);
                        int steepness = Math.Max(Math.Max(Math.Abs(northwest), Math.Abs(north)), Math.Abs(west));
                        float slopeFactor = Math.Min(0.5F, steepness / 10F) / 1.25F;
                        float yDiff = direction switch {
                            > 0 => 1.08F + slopeFactor,
                            < 0 => 0.92F - slopeFactor,
                            _ => 1
                        };

                        image.SetBlockColor(imgX, imgZ, color, yDiff);
                    } catch (Exception) {
                        image.SetBlockColor(imgX, imgZ, 0, 0);
                    }
                }
            }
        }
    }

    private int GetTopBlockY(ServerMapChunk mapChunk, int x, int z) {
        return GameMath.Clamp(mapChunk.RainHeightMap[GetChunkIndex(x, z)], 0, Api.WorldManager.MapSizeY - 1);
    }

    private int GetBlockColor(string block, int x, int y, int z) {
        int[]? colors = Colormap?.Get(block);
        if (colors == null) {
            Logger.Warn($"No color for block ({block})");
        }

        return (colors == null ? 0xFF << 16 : colors[GameMath.MurmurHash3Mod(x, y, z, 30)]) | 0xFF << 24;
        // todo - test all blocks for colors on start and warn only once
    }

    private static int GetChunkIndex(int x, int z) {
        return ((z & 31) << 5) + (x & 31);
    }

    private static int GetChunkIndex(int x, int y, int z) {
        return ((((y & 31) << 5) + z) << 5) + x;
    }
}
