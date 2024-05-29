using System.Linq;
using JetBrains.Annotations;

namespace livemap.util;

[PublicAPI]
public abstract class Mathf {
    public static long AsLong(int x, int z) {
        return (x & 0xFFFFFFFF) | (z & 0xFFFFFFFF) << 32;
    }

    public static int LongToX(long index) {
        return (int)(index & 0xFFFFFFFF);
    }

    public static int LongToZ(long index) {
        return (int)(index >>> 32 & 0xFFFFFFFF);
    }

    public static int BlockIndex(int x, int z) {
        return ((z & 31) << 5) + (x & 31);
    }

    public static int BlockIndex(int x, int y, int z) {
        return ((((y & 31) << 5) + (z & 31)) << 5) + (x & 31);
    }

    public static float Lerp(float a, float b, float t) {
        return a + (t * (b - a));
    }

    public static uint Max(params uint[] numbers) {
        return numbers.Max();
    }

    public static uint Min(params uint[] numbers) {
        return numbers.Min();
    }
}
