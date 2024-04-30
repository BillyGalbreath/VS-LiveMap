using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace livemap.common.render;

[PublicAPI]
public class BlockData {
    private readonly Data[] _data = new Data[512 * 512];
    private readonly Data[] _north = new Data[512];
    private readonly Data[] _east = new Data[512];
    private readonly Data[] _south = new Data[512];
    private readonly Data[] _west = new Data[512];
    private Data? _northwest;
    private Data? _northeast;
    private Data? _southeast;
    private Data? _southwest;

    [SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
    public Data? Get(int x, int z) {
        if (x < 0 && z < 0) {
            return _northwest;
        }
        if (x > 511 && z < 0) {
            return _northeast;
        }
        if (x > 511 && z > 511) {
            return _southeast;
        }
        if (x < 0 && z > 511) {
            return _southwest;
        }
        if (z < 0) {
            return _north.GetValue(x) as Data;
        }
        if (x > 511) {
            return _east.GetValue(z) as Data;
        }
        if (z > 511) {
            return _south.GetValue(x) as Data;
        }
        if (x < 0) {
            return _west.GetValue(z) as Data;
        }
        return _data.GetValue(Index(x, z)) as Data;
    }

    [SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
    public void Set(int x, int z, Data data) {
        if (x < 0 && z < 0) {
            _northwest = data;
        } else if (x > 511 && z < 0) {
            _northeast = data;
        } else if (x > 511 && z > 511) {
            _southeast = data;
        } else if (x < 0 && z > 511) {
            _southwest = data;
        } else if (z < 0) {
            _north[x] = data;
        } else if (x > 511) {
            _east[z] = data;
        } else if (z > 511) {
            _south[x] = data;
        } else if (x < 0) {
            _west[z] = data;
        } else {
            _data[Index(x, z)] = data;
        }
    }

    private static int Index(int x, int z) {
        return ((z & 511) * 512) + (x & 511);
    }

    public class Data {
        public int Y { get; }
        public int Top { get; }
        public int Under { get; }

        public Data(int y, int top, int under) {
            Y = y;
            Top = top;
            Under = under;
        }
    }
}
