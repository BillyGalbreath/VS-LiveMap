﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using LiveMap.Common.Util;
using LiveMap.Server.Util;
using SkiaSharp;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace LiveMap.Server.Render;

public abstract class Renderer {
    private readonly RenderTask renderTask;
    private readonly Random rand;
    private readonly MethodInfo cachedBlurMethod;

    private Colormap? Colormap => renderTask.Server.Colormap;
    private ICoreServerAPI Api => renderTask.Server.Api;

    protected Renderer(RenderTask renderTask) {
        this.renderTask = renderTask;
        rand = new Random();
        cachedBlurMethod = typeof(ChunkMapLayer).Assembly
            .GetType("Vintagestory.GameContent.BlurTool")!
            .GetMethod("Blur", BindingFlags.Public | BindingFlags.Static)!;
    }

    public void ScanRegion(long region) {
        int regionX = Mathf.LongToX(region);
        int regionZ = Mathf.LongToZ(region);

        Logger.Debug($">>> Scanning Region {regionX},{regionZ} >>>");

        int chunkX = regionX << 5;
        int chunkZ = regionZ << 5;

        ManualResetEvent mre = new(false);

        Api.WorldManager.LoadChunkColumnPriority(chunkX, chunkZ, chunkX + 16, chunkZ + 16,
            new ChunkLoadOptions {
                OnLoaded = () => {
                    try {
                        ScanRegion(regionX, regionZ);
                    } catch (Exception e) {
                        Logger.Error(e.ToString());
                    }

                    mre.Set();
                }
            }
        );

        mre.WaitOne();
    }

    private unsafe void ScanRegion(int regionX, int regionZ) {
        if (renderTask.Stopped) {
            return;
        }

        SKBitmap png = new(512, 512);
        byte* pngPtr = (byte*)png.GetPixels().ToPointer();
        byte[] shadowMap = new byte[512 << 9].Fill((byte)128);

        int startBlockX = regionX << 9;
        int startBlockZ = regionZ << 9;
        int endBlockX = startBlockX + 512;
        int endBlockZ = startBlockZ + 512;

        for (int blockX = startBlockX; blockX < endBlockX; blockX++) {
            for (int blockZ = startBlockZ; blockZ < endBlockZ; blockZ++) {
                if (renderTask.Stopped) {
                    return;
                }

                if (blockX < 0 || blockX > Api.WorldManager.MapSizeX) {
                    continue;
                }

                if (blockZ < 0 || blockZ > Api.WorldManager.MapSizeZ) {
                    continue;
                }

                try {
                    int blockY = GetYFromRainMap(blockX, blockZ);
                    if (blockY < 0 || blockY > Api.WorldManager.MapSizeY) {
                        continue;
                    }

                    BlockPos pos = new(blockX, blockY, blockZ);
                    Block? block = GetBlockToRender(pos);
                    if (block == null) {
                        continue;
                    }

                    int imgX = blockX & 511;
                    int imgZ = blockZ & 511;

                    uint* row = (uint*)(pngPtr + imgZ * png.RowBytes);
                    row[imgX] = (uint)GetBlockColor(GetBlockFromDecor(block, pos).Code.ToString());

                    shadowMap[(imgZ << 9) + imgX] = (byte)(shadowMap[(imgZ << 9) + imgX] * CalculateAltitudeDiff(pos));
                } catch (Exception) {
                    // ignored
                }
            }
        }

        if (renderTask.Stopped) {
            return;
        }

        byte[] shadowMapCopy = shadowMap.ToArray();

        cachedBlurMethod.Invoke(null, new object[] { shadowMap, 512, 512, 2 });

        for (int i = 0; i < shadowMap.Length; i++) {
            if (renderTask.Stopped) {
                return;
            }

            float shadow = (int)((shadowMap[i] / 128F - 1F) * 5F) / 5F;
            shadow += (shadowMapCopy[i] / 128F - 1F) * 5F % 1F / 5F;

            int imgX = i & 511;
            int imgZ = i >> 9;

            uint* row = (uint*)(pngPtr + imgZ * png.RowBytes);
            row[imgX] = (uint)(row[imgX] == 0
                ? 0
                : ColorUtil.ColorMultiply3Clamped((int)row[imgX], shadow * 1.4F + 1F) | 0xFF << 24);
        }

        if (renderTask.Stopped) {
            return;
        }

        int localRegionX = (startBlockX - (Api.WorldManager.MapSizeX >> 1)) >> 9;
        int localRegionZ = (startBlockZ - (Api.WorldManager.MapSizeZ >> 1)) >> 9;

        FileInfo fileInfo = new(Path.Combine(FileUtil.TilesDir, 0.ToString(), $"{localRegionX}_{localRegionZ}.png"));
        GamePaths.EnsurePathExists(fileInfo.Directory!.FullName);

        using FileStream fileStream = fileInfo.Create();
        png.Encode(SKEncodedImageFormat.Png, 100).SaveTo(fileStream);
        png.Dispose();
    }

    private int GetYFromRainMap(int x, int z) {
        IServerMapChunk? chunk = Api.WorldManager.GetMapChunk(x >> 5, z >> 5);
        return chunk?.RainHeightMap[((z & 31) << 5) + (x & 31)] ?? Api.WorldManager.MapSizeY;
    }

    private Block? GetBlockToRender(BlockPos pos) {
        IWorldChunk? chunk = Api.WorldManager.GetChunk(pos.X >> 5, pos.Y >> 5, pos.Z >> 5);
        if (chunk == null) {
            return null;
        }

        int blockId = chunk.UnpackAndReadBlock(MapUtil.Index3d(pos.X & 31, pos.Y & 31, pos.Z & 31, 32, 32), 3);
        return blockId == 0 ? null : Api.World.Blocks[blockId];
    }

    private int GetBlockColor(string block) {
        int[]? colors = Colormap?.Get(block);
        return (colors == null ? 0xFF << 16 : colors[rand.Next(30)]) | 0xFF << 24;
        // todo - test all blocks for colors on start and warn only once
    }

    [SuppressMessage("ReSharper", "TailRecursiveCall")]
    private Block GetBlockFromDecor(Block block, BlockPos pos) {
        Block? decor = block.HasBehavior("Decor", Api.ClassRegistry)
            ? null
            : Api.World.BlockAccessor.GetDecor(pos, BlockFacing.UP.Index);
        return decor == null || decor == block ? block : GetBlockFromDecor(decor, pos);
    }

    private float CalculateAltitudeDiff(BlockPos pos) {
        int offsetX = pos.X - 1;
        int offsetZ = pos.Z - 1;

        BlockPos delta = new(offsetX, GetYFromRainMap(offsetX, offsetZ), offsetZ);
        int leftTop = pos.Y - (GetBlockToRender(delta) == null ? pos.Y : delta.Y);

        delta.Set(pos.X, GetYFromRainMap(pos.X, offsetZ), offsetZ);
        int rightTop = pos.Y - (GetBlockToRender(delta) == null ? pos.Y : delta.Y);

        delta.Set(offsetX, GetYFromRainMap(offsetX, pos.Z), pos.Z);
        int leftBot = pos.Y - (GetBlockToRender(delta) == null ? pos.Y : delta.Y);

        int direction = Math.Sign(leftTop) + Math.Sign(rightTop) + Math.Sign(leftBot);
        int steepness = Math.Max(Math.Max(Math.Abs(leftTop), Math.Abs(rightTop)), Math.Abs(leftBot));
        float slopeFactor = Math.Min(0.5F, steepness / 10F) / 1.25F;
        return direction switch {
            > 0 => 1.08F + slopeFactor,
            < 0 => 0.92F - slopeFactor,
            _ => 1
        };
    }
}
