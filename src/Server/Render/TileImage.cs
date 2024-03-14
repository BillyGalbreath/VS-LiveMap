using System.IO;
using System.Linq;
using LiveMap.Common.Util;
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

    public void SetBlockColor(int blockX, int blockZ, int color, float yDiff) {
        int imgX = blockX & 511;
        int imgZ = blockZ & 511;

        uint* row = (uint*)(_pngPtr + imgZ * _pngRowBytes);
        row[imgX] = (uint)color;

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
            row[imgX] = (uint)(row[imgX] == 0 ? 0 : ColorUtil.ColorMultiply3Clamped((int)row[imgX], shadow * 1.4F + 1F) | 0xFF << 24);
        }
    }

    public void SetPixels(int[] pixels) {
        for (int i = 0; i < pixels.Length; i++) {
            int x = i % 512;
            int z = i / 512;
            uint* row = (uint*)(_pngPtr + z * _pngRowBytes);
            row[x] = (uint)pixels[i];
        }
    }

    public void Save() {
        FileInfo fileInfo = new(Path.Combine(FileUtil.TilesDir, 0.ToString(), $"{_regionX}_{_regionZ}.png"));
        GamePaths.EnsurePathExists(fileInfo.Directory!.FullName);

        using FileStream fileStream = fileInfo.Create();
        _png.Encode(SKEncodedImageFormat.Png, 100).SaveTo(fileStream);
        _png.Dispose();
    }
}
