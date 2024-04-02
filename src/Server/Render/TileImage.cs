using System;
using System.IO;
using System.Linq;
using LiveMap.Common.Util;
using LiveMap.Server.Configuration;
using SkiaSharp;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace LiveMap.Server.Render;

public sealed unsafe class TileImage {
    private readonly SKBitmap _png;
    private readonly byte* _pngPtr;
    private readonly byte[] _shadowMap;

    private readonly int _pngRowBytes;

    private readonly int _regionX;
    private readonly int _regionZ;

    public TileImage(int regionX, int regionZ) {
        _png = new SKBitmap(512, 512);
        _pngPtr = (byte*)_png.GetPixels().ToPointer();
        _shadowMap = new byte[512 << 9].Fill((byte)128);

        _pngRowBytes = _png.RowBytes;

        _regionX = regionX;
        _regionZ = regionZ;
    }

    public void SetBlockColor(int blockX, int blockZ, uint argb, float yDiff) {
        int imgX = blockX & 511;
        int imgZ = blockZ & 511;

        ((uint*)(_pngPtr + imgZ * _pngRowBytes))[imgX] = argb;

        _shadowMap[(imgZ << 9) + imgX] = (byte)(_shadowMap[(imgZ << 9) + imgX] * yDiff);
    }

    public void CalculateShadows() {
        byte[] shadowMapCopy = _shadowMap.ToArray();
        BlurTool.Blur(_shadowMap, 512, 512, 2);
        for (int i = 0; i < _shadowMap.Length; i++) {
            float shadow = (int)((_shadowMap[i] / 128F - 1F) * 5F) / 5F;
            shadow += (shadowMapCopy[i] / 128F - 1F) * 5F % 1F / 5F;

            int imgX = i & 511;
            int imgZ = i >> 9;

            uint* row = (uint*)(_pngPtr + imgZ * _pngRowBytes);
            row[imgX] = (uint)(row[imgX] == 0 ? 0 : ColorUtil.ColorMultiply3Clamped((int)row[imgX], shadow * 1.4F + 1F));
        }
    }

    public void Save() {
        try {
            for (int zoom = 0; zoom <= Config.Instance.Zoom.MaxOut; zoom++) {
                FileInfo fileInfo = new(Path.Combine(FileUtil.TilesDir, zoom.ToString(), $"{_regionX >> zoom}_{_regionZ >> zoom}.png"));
                GamePaths.EnsurePathExists(fileInfo.Directory!.FullName);

                if (zoom > 0) {
                    using FileStream fileStream1 = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                    SKBitmap png = SKBitmap.Decode(fileStream1) ?? new SKBitmap(512, 512);

                    WritePixels(png, zoom);

                    using FileStream fileStream2 = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                    png.Encode(SKEncodedImageFormat.Png, 0).SaveTo(fileStream2);
                    png.Dispose();
                } else {
                    using FileStream fileStream = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                    _png.Encode(SKEncodedImageFormat.Png, 0).SaveTo(fileStream);
                }
            }

            _png.Dispose();
        } catch (Exception e) {
            Logger.Error(e.ToString());
        }
    }

    private void WritePixels(SKBitmap png, int zoom) {
        int step = 1 << zoom;
        int baseX = (_regionX * 512 >> zoom) & 511;
        int baseZ = (_regionZ * 512 >> zoom) & 511;
        byte* pngPtr = (byte*)png.GetPixels().ToPointer();
        int pngRowBytes = png.RowBytes;
        for (int x = 0; x < 512; x += step) {
            for (int z = 0; z < 512; z += step) {
                uint argb = ((uint*)(_pngPtr + z * _pngRowBytes))[x];
                if (argb == 0) {
                    // skipping 0 prevents overwrite existing
                    // parts of the buffer of existing images
                    continue;
                }

                if (step > 1) {
                    // merge pixel colors instead of skipping them
                    argb = DownSample(x, z, argb, step);
                }

                ((uint*)(pngPtr + (baseZ + (z >> zoom)) * pngRowBytes))[baseX + (x >> zoom)] = argb;
            }
        }
    }

    private uint DownSample(int x, int z, uint argb, int step) {
        uint a = 0, r = 0, g = 0, b = 0, c = 0;
        for (int i = 0; i < step; i++) {
            for (int j = 0; j < step; j++) {
                if (i != 0 && j != 0) {
                    argb = ((uint*)(_pngPtr + (z + j) * _pngRowBytes))[x + i];
                }

                a += argb >> 24 & 0xFF;
                r += argb >> 16 & 0xFF;
                g += argb >> 8 & 0xFF;
                b += argb >> 0 & 0xFF;
                c++;
            }
        }

        return c == 0 ? 0 : (a / c) << 24 | (r / c) << 16 | (g / c) << 8 | (b / c);
    }
}
