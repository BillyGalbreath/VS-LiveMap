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
}
