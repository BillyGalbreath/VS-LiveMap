using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LiveMap.Common.Util;

public sealed class Colormap {
    [YamlMember] private readonly Dictionary<string, int[]> _colors = new();

    public void Add(string block, int[] toAdd) {
        _colors.TryAdd(block, toAdd);
    }

    public Dictionary<int, int[]> ToDict(ICoreAPI api) {
        Dictionary<int, int[]> dict = new();
        foreach ((string code, int[] colors) in _colors) {
            int? id = api.World.GetBlock(new AssetLocation(code))?.Id;
            if (id != null) {
                dict.TryAdd((int)id, colors);
            }
        }

        return dict;
    }

    public string Serialize() {
        return new SerializerBuilder()
            .WithQuotingNecessaryStrings()
            .WithNamingConvention(NullNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build()
            .Serialize(_colors);
    }

    public static Colormap Deserialize(string yaml) {
        Dictionary<string, int[]> data = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithNamingConvention(NullNamingConvention.Instance)
            .Build().Deserialize<Dictionary<string, int[]>>(yaml);

        Colormap colormap = new();
        foreach ((string? key, int[]? value) in data) {
            colormap._colors.TryAdd(key, value);
        }

        return colormap;
    }

    public static Colormap? Read() {
        try {
            string yaml = File.ReadAllText(FileUtil.ColormapFile, Encoding.UTF8);
            if (!string.IsNullOrEmpty(yaml)) {
                Colormap colormap = Deserialize(yaml);
                Logger.Info(Lang.Get("logger.info.server-loaded-colormap"));
                return colormap;
            }
        } catch (Exception) {
            // ignore
        }

        Logger.Warn("Could not load colormap from disk.");
        Logger.Warn("An admin needs to send the colormap from their client.");
        return null;
    }

    public static Colormap Write(Colormap colormap) {
        GamePaths.EnsurePathExists(FileUtil.DataDir);
        File.WriteAllText(FileUtil.ColormapFile, colormap.Serialize(), Encoding.UTF8);
        return colormap;
    }

    public void Dispose() {
        _colors.Clear();
    }
}
