namespace LiveMap.Server;

public class Region {
    public int PosX { get; }
    public int PosZ { get; }

    public long Index { get; }

    public Region(int posX, int posZ) : this(posX, posZ, AsLong(posX, posZ)) {
    }

    public Region(long index) : this(LongToX(index), LongToZ(index), index) {
    }

    private Region(int posX, int posZ, long index) {
        PosX = posX;
        PosZ = posZ;

        Index = index;
    }

    private static long AsLong(long x, long z) {
        return (x & 0xFFFFFFFFL) | (z & 0xFFFFFFFFL) << 32;
    }

    private static int LongToX(long pos) {
        return (int)(pos & 0xFFFFFFFFL);
    }

    private static int LongToZ(long pos) {
        return (int)(pos >>> 32 & 0xFFFFFFFFL);
    }
}
