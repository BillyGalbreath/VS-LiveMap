namespace LiveMap.Common.Util;

public static class Mathf {
    public static long AsLong(long x, long z) {
        return (x & 0xFFFFFFFFL) | (z & 0xFFFFFFFFL) << 32;
    }

    public static int LongToX(long pos) {
        return (int)(pos & 0xFFFFFFFFL);
    }

    public static int LongToZ(long pos) {
        return (int)(pos >>> 32 & 0xFFFFFFFFL);
    }
}
