namespace livemap.render;

public class BlockData {
    private readonly Data[] _data = new Data[512 * 512];

    public Data? Get(int x, int z) {
        if (x is < 0 or > 511 || z is < 0 or > 511) {
            // todo - i really want to get the edge data from the neighbor regions..
            return null;
        }

        return _data.GetValue(Index(x, z)) as Data;
    }

    public void Set(int x, int z, Data data) {
        _data[Index(x, z)] = data;
    }

    private static int Index(int x, int z) {
        return ((z & 511) * 512) + (x & 511);
    }

    public class Data(int y, int top, int under) {
        public int Y { get; } = y;
        public int Top { get; } = top;
        public int Under { get; } = under;
        public Dictionary<string, object?> Custom = [];
    }
}
