using System;
using System.Reflection;

namespace LiveMap.Common.Extensions;

public static class ReflectionExtensions {
    private const BindingFlags _flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private static Type GetDeclaredType<T>(this T? obj) {
        return obj != null ? obj.GetType() : typeof(T);
    }

    public static T? GetField<T>(this object? obj, string name) where T : class {
        return obj.GetDeclaredType().GetField(name, _flags)?.GetValue(obj) as T;
    }

    public static void SetField(this object? obj, string name, object? val) {
        obj.GetDeclaredType().GetField(name, _flags)?.SetValue(obj, val);
    }
}
