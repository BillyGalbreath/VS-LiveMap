using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using JetBrains.Annotations;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;

namespace livemap.common.util;

[PublicAPI]
public static class Extensions {
    private const BindingFlags _flags = BindingFlags.NonPublic | BindingFlags.Instance;

    public static T? GetField<T>(this object obj, string name) where T : class {
        return obj.GetType().GetField(name, _flags)?.GetValue(obj) as T;
    }

    public static void AddIfNotExists<T>(this ICollection<T> collection, T value) {
        if (!collection.Contains(value)) {
            collection.Add(value);
        }
    }

    public static string ToLang(this string key, params object[]? args) {
        return Lang.Get($"{LiveMapMod.Id}:{key}", args);
    }

    public static Point GetPoint(this IPlayer player) {
        return player.Entity.GetPoint();
    }

    public static Point GetPoint(this EntityPlayer player) {
        EntityPos pos = player.SidedPos;
        return new Point(pos.X, pos.Z);
    }

    public static uint ToColor(this Vector4 vec) {
        int b = (int)(vec.X * 0xFF);
        int g = (int)(vec.Y * 0xFF);
        int r = (int)(vec.Z * 0xFF);
        int a = (int)(vec.W * 0xFF);
        return (uint)(a << 24 | r << 16 | g << 8 | b);
    }

    public static Direction Next(this Direction direction) {
        return (Direction)(((int)direction + 1) & 3);
    }
}
