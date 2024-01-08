using LiveMap.Common.Util;

namespace LiveMap.Server;

public class Region {
    public int PosX { get; }
    public int PosZ { get; }

    public long Index { get; }

    public Region(int posX, int posZ) : this(posX, posZ, Mathf.AsLong(posX, posZ)) {
    }

    public Region(long index) : this(Mathf.LongToX(index), Mathf.LongToX(index), index) {
    }

    private Region(int posX, int posZ, long index) {
        PosX = posX;
        PosZ = posZ;

        Index = index;
    }
}
