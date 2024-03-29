namespace LiveMap.Common.Util;

public static class Mathf {
    public static long AsLong(int x, int z) {
        return (x & 0xFFFFFFFF) | (z & 0xFFFFFFFF) << 32;
    }

    public static int LongToX(long index) {
        return (int)(index & 0xFFFFFFFF);
    }

    public static int LongToZ(long index) {
        return (int)(index >>> 32 & 0xFFFFFFFF);
    }

    public static int AsIndex(int x, int z) {
        // z % 32 * 32 + x % 32
        return ((z & 31) << 5) + (x & 31);
    }

    public static int AsIndex(int x, int y, int z) {
        // (y % 32 * 32 + z) * 32 + x;
        return ((((y & 31) << 5) + z) << 5) + x;
    }
}
