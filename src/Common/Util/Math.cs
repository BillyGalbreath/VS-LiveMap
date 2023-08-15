namespace LiveMap.src.Common.Util;

public class Math {
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
