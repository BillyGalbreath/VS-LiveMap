namespace livemap.common.util;

public static class Color {
    public static int Reverse(int abgr) {
        return abgr & 0xFF << 24 |
               (abgr >> 16 & 0xFF) << 0 |
               (abgr >> 8 & 0xFF) << 8 |
               (abgr >> 0 & 0xFF) << 16;
    }

    public static int Blend(int argb1, int argb2) {
        return ((argb1 >> 24 & 0xFF) << 24) |
               (int)(((argb1 >> 16 & 0xFF) * 0.4) + ((argb2 >> 16 & 0xFF) * 0.6)) << 16 |
               (int)(((argb1 >> 8 & 0xFF) * 0.4) + ((argb2 >> 8 & 0xFF) * 0.6)) << 8 |
               (int)(((argb1 >> 0 & 0xFF) * 0.4) + ((argb2 >> 0 & 0xFF) * 0.6));
    }
}
