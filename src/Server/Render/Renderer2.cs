using System;
using System.Threading;
using LiveMap.Common.Util;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace LiveMap.Server.Render;

public abstract class Renderer2 {
    private readonly RenderTask _renderTask;
    private readonly Random _rand;

    private Colormap? Colormap => _renderTask.Server.Colormap;
    private ICoreServerAPI Api => _renderTask.Server.Api;

    protected Renderer2(RenderTask renderTask) {
        _renderTask = renderTask;
        _rand = new Random();
    }

    public void ScanRegion(long region) {
        int regionX = Mathf.LongToX(region);
        int regionZ = Mathf.LongToZ(region);

        Logger.Debug($"&3Scanning Region {regionX},{regionZ} (thread: {Environment.CurrentManagedThreadId})");

        try {
            // allocate image
            TileImage image = new(regionX, regionZ);

            // scan the region
            ScanRegion(image, regionX, regionZ);

            // calculate shadows
            image.CalculateShadows();

            // save image
            image.Save();
        } catch (Exception e) {
            Logger.Error(e.ToString());
        }

        // give the cpu a rest between regions
        Thread.Sleep(500);

        Logger.Debug($"&aFinished Region {regionX},{regionZ} (thread: {Environment.CurrentManagedThreadId})");
    }

    private void ScanRegion(TileImage image, int regionX, int regionZ) {
        int startChunkX = regionX << 4;
        int startChunkZ = regionZ << 4;
        int endChunkX = startChunkX + 16;
        int endChunkZ = startChunkZ + 16;

        /*for (int chunkX = startChunkX; chunkX < endChunkX; chunkX++) {
            for (int chunkZ = startChunkZ; chunkZ < endChunkZ; chunkZ++) {
                ScanChunk(image, chunkX, chunkZ);
            }
        }*/

        ManualResetEvent mre = new(false);

        // make sure chunk is actually loaded
        Api.WorldManager.LoadChunkColumnPriority(startChunkX, startChunkZ, endChunkX, endChunkZ,
            new ChunkLoadOptions {
                OnLoaded = () => {
                    // chan the chunks
                    for (int chunkX = startChunkX; chunkX < endChunkX; chunkX++) {
                        for (int chunkZ = startChunkZ; chunkZ < endChunkZ; chunkZ++) {
                            ScanChunkColumn(image, chunkX, chunkZ);
                        }
                    }

                    // continue thread
                    mre.Set();
                }
            }
        );

        // hold the thread until the above task tells us it's done
        mre.WaitOne();
    }

    private void ScanChunk(TileImage image, int chunkX, int chunkZ) {
        ManualResetEvent mre = new(false);

        // make sure chunk is actually loaded
        Api.WorldManager.LoadChunkColumnPriority(chunkX, chunkZ,
            new ChunkLoadOptions {
                OnLoaded = () => {
                    // chan the chunk
                    ScanChunkColumn(image, chunkX, chunkZ);
                    // continue thread
                    mre.Set();
                }
            }
        );

        // hold the thread until the above task tells us it's done
        mre.WaitOne();
    }

    private void ScanChunkColumn(TileImage image, int chunkX, int chunkZ) {
        int startBlockX = chunkX << 5;
        int startBlockZ = chunkZ << 5;
        int endBlockX = startBlockX + 32;
        int endBlockZ = startBlockZ + 32;

        for (int blockX = startBlockX; blockX < endBlockX; blockX++) {
            for (int blockZ = startBlockZ; blockZ < endBlockZ; blockZ++) {
                if (blockX < 0 || blockX > Api.WorldManager.MapSizeX) {
                    return;
                }

                if (blockZ < 0 || blockZ > Api.WorldManager.MapSizeZ) {
                    return;
                }

                int blockY = GetYFromRainMap(blockX, blockZ);
                if (blockY < 0 || blockY > Api.WorldManager.MapSizeY) {
                    return;
                }

                BlockPos pos = new(blockX, blockY, blockZ, 0);
                Block? block = GetBlockToRender(pos);
                if (block == null) {
                    return;
                }

                int color = GetBlockColor(block.Code.ToString());
                float yDiff = CalculateAltitudeDiff(pos);

                image.SetBlockColor(blockX, blockZ, color, yDiff);
            }
        }
    }

    private int GetYFromRainMap(int x, int z) {
        IServerMapChunk? mapChunk = Api.WorldManager.GetMapChunk(x >> 5, z >> 5);
        return mapChunk?.RainHeightMap[((z & 31) << 5) + (x & 31)] ?? Api.WorldManager.MapSizeY;
    }

    private Block? GetBlockToRender(BlockPos pos) {
        IWorldChunk? chunk = Api.World.GetCachingBlockAccessor(false, false).GetChunk(pos.X >> 5, pos.Y >> 5, pos.Z >> 5);
        if (chunk == null) {
            return null;
        }

        int blockId = chunk.UnpackAndReadBlock(MapUtil.Index3d(pos.X & 31, pos.Y & 31, pos.Z & 31, 32, 32), 3);
        return blockId == 0 ? null : Api.World.Blocks[blockId];
    }

    private int GetBlockColor(string block) {
        int[]? colors = Colormap?.Get(block);
        if (colors == null) {
            Logger.Warn($"No color for block ({block})");
        }

        return (colors == null ? 0xFF << 16 : colors[_rand.Next(30)]) | 0xFF << 24;
        // todo - test all blocks for colors on start and warn only once
    }

    private Block GetBlockFromDecor(Block block, BlockPos pos) {
        while (true) {
            Block? decor = block.HasBehavior("Decor", Api.ClassRegistry) ? null : Api.World.GetCachingBlockAccessor(false, false).GetDecor(pos, BlockFacing.UP.Index);
            if (decor == null || decor == block) {
                return block;
            }

            block = decor;
        }
    }

    private float CalculateAltitudeDiff(BlockPos pos) {
        int offsetX = pos.X - 1;
        int offsetZ = pos.Z - 1;

        BlockPos delta = new(offsetX, GetYFromRainMap(offsetX, offsetZ), offsetZ, 0);
        int northwest = pos.Y - (GetBlockToRender(delta) == null ? pos.Y : delta.Y);

        delta.Set(pos.X, GetYFromRainMap(pos.X, offsetZ), offsetZ);
        int north = pos.Y - (GetBlockToRender(delta) == null ? pos.Y : delta.Y);

        delta.Set(offsetX, GetYFromRainMap(offsetX, pos.Z), pos.Z);
        int west = pos.Y - (GetBlockToRender(delta) == null ? pos.Y : delta.Y);

        int direction = Math.Sign(northwest) + Math.Sign(north) + Math.Sign(west);
        int steepness = Math.Max(Math.Max(Math.Abs(northwest), Math.Abs(north)), Math.Abs(west));
        float slopeFactor = Math.Min(0.5F, steepness / 10F) / 1.25F;
        return direction switch {
            > 0 => 1.08F + slopeFactor,
            < 0 => 0.92F - slopeFactor,
            _ => 1
        };
    }
}
