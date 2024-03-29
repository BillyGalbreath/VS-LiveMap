using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LiveMap.Common.Util;

public sealed class Colormap {
    [YamlMember] private readonly Dictionary<string, int[]> _colorsByName = new();

    private readonly Dictionary<int, int[]> _colorsById = new();

    public void Add(string block, int[] toAdd) {
        _colorsByName.TryAdd(block, toAdd);
    }

    public bool TryGet(int id, [MaybeNullWhen(false)] out int[] colors) {
        return _colorsById.TryGetValue(id, out colors);
        //Logger.Warn("Unable to scan regions. No known colormap detected.");
        //Logger.Warn("An admin needs to send the colormap from their client.");
    }

    public string Serialize() {
        return new SerializerBuilder()
            .WithQuotingNecessaryStrings()
            .WithNamingConvention(NullNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build()
            .Serialize(_colorsByName);
    }

    public static Colormap Deserialize(string yaml) {
        Dictionary<string, int[]> data = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithNamingConvention(NullNamingConvention.Instance)
            .Build().Deserialize<Dictionary<string, int[]>>(yaml);

        Colormap colormap = new();
        foreach ((string? key, int[]? value) in data) {
            colormap._colorsByName.TryAdd(key, value);
        }

        return colormap;
    }

    public void Reload(ICoreAPI api) {
        _colorsByName.Clear();
        _colorsById.Clear();

        try {
            string yaml = File.ReadAllText(FileUtil.ColormapFile, Encoding.UTF8);
            if (!string.IsNullOrEmpty(yaml)) {
                _colorsByName.AddRange(Deserialize(yaml)._colorsByName);
                RefreshIds(api.World);
                Logger.Info(Lang.Get("logger.info.server-loaded-colormap"));
                return;
            }
        } catch (Exception) {
            // ignore
        }

        Logger.Warn("Could not load colormap from disk.");
        Logger.Warn("An admin needs to send the colormap from their client.");
    }

    public void Write() {
        GamePaths.EnsurePathExists(FileUtil.DataDir);
        File.WriteAllText(FileUtil.ColormapFile, Serialize(), Encoding.UTF8);
    }

    public void RefreshIds(IWorldAccessor world) {
        _colorsById.Clear();

        foreach ((string code, int[] colors) in _colorsByName) {
            int? id = world.GetBlock(new AssetLocation(code))?.Id;
            if (id != null) {
                _colorsById.TryAdd((int)id, colors);
            }
        }
    }

    public void Dispose() {
        _colorsByName.Clear();
    }
}
