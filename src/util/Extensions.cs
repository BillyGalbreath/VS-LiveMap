using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using JetBrains.Annotations;
using livemap.data;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace livemap.util;

[PublicAPI]
public static class Extensions {
    private const BindingFlags _flags = BindingFlags.NonPublic | BindingFlags.Instance;

    public static T? GetField<T>(this object obj, string name) where T : class {
        return obj.GetType().GetField(name, _flags)?.GetValue(obj) as T;
    }

    public static V ComputeIfAbsent<K, V>(this Dictionary<K, V> dict, K key, System.Func<K, V> func) where K : notnull {
        if (!dict.TryGetValue(key, out V? value)) {
            value = func.Invoke(key);
            dict.Add(key, value);
        }
        return value;
    }

    public static void AddIfNotExists<T>(this List<T> list, T value) {
        if (!list.Contains(value)) {
            list.Add(value);
        }
    }

    public static string ToLang(this string key, params object[]? args) {
        return Lang.Get($"{LiveMap.Api.ModId}:{key}", args);
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

    public static Point ToPoint(this BlockPos pos) {
        return new Point(pos.X, pos.Z);
    }

    public static Point ToPoint(this EntityPos pos) {
        return new Point(Math.Round(pos.X, 1), Math.Round(pos.Z, 1));
    }

    public static Point Size(this IWorldManagerAPI api) {
        return new Point(api.MapSizeX, api.MapSizeZ);
    }

    public static Dictionary<string, object> GetHealth(this IPlayer player) {
        EntityBehaviorHealth health = player.Entity.GetBehavior<EntityBehaviorHealth>();
        return new Dictionary<string, object> {
            { "cur", health?.Health ?? 15 },
            { "max", health?.MaxHealth ?? 15 }
        };
    }

    public static Dictionary<string, object> GetSatiety(this IPlayer player) {
        EntityBehaviorHunger satiety = player.Entity.GetBehavior<EntityBehaviorHunger>();
        return new Dictionary<string, object> {
            { "cur", satiety?.Saturation ?? 1500 },
            { "max", satiety?.MaxSaturation ?? 1500 }
        };
    }

    public static string GetAvatar(this EntityPlayer player) {
        ITreeAttribute appliedParts = (ITreeAttribute)player.WatchedAttributes.GetTreeAttribute("skinConfig")["appliedParts"];
        return $"https://vs.pl3x.net/v1/" +
               $"{appliedParts.GetString("baseskin")}/" +
               $"{appliedParts.GetString("eyecolor")}/" +
               $"{appliedParts.GetString("hairbase")}/" +
               $"{appliedParts.GetString("hairextra")}/" +
               $"{appliedParts.GetString("mustache")}/" +
               $"{appliedParts.GetString("beard")}/" +
               $"{appliedParts.GetString("haircolor")}.png";
    }
}
