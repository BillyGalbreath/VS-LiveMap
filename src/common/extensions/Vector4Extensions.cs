using System.Numerics;

namespace livemap.common.extensions;

public static class Vector4Extensions {
    public static uint ToColor(this Vector4 vec) {
        int b = (int)(vec.X * 0xFF);
        int g = (int)(vec.Y * 0xFF);
        int r = (int)(vec.Z * 0xFF);
        int a = (int)(vec.W * 0xFF);
        return (uint)(a << 24 | r << 16 | g << 8 | b);
    }
}
