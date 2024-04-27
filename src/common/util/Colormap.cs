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

namespace livemap.common.util;

public sealed class Colormap {
    [YamlMember]
    private readonly Dictionary<string, uint[]> _colorsByName = new();

    private readonly Dictionary<int, uint[]> _colorsById = new();

    public void Add(string block, uint[] toAdd) {
        _colorsByName.TryAdd(block, toAdd);
    }

    public bool TryGet(int id, [MaybeNullWhen(false)] out uint[] colors) {
        return _colorsById.TryGetValue(id, out colors);
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
        Dictionary<string, uint[]> data = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .WithNamingConvention(NullNamingConvention.Instance)
            .Build().Deserialize<Dictionary<string, uint[]>>(yaml);

        Colormap colormap = new();
        foreach ((string? key, uint[]? colors) in data) {
            colormap._colorsByName.TryAdd(key, colors);
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
                Logger.Info("&dColormap loaded from disk.");
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

        foreach ((string code, uint[] colors) in _colorsByName) {
            Block block = world.GetBlock(new AssetLocation(code));
            if (block == null) {
                Logger.Warn($"Invalid block id in colormap ({code})");
                continue;
            }

            // fix alpha channel here
            for (int i = 0; i < colors.Length; i++) {
                if (colors[i] > 0) {
                    colors[i] |= (uint)0xFF << 24;
                }
            }

            _colorsById.TryAdd(block.Id, colors);
        }
    }

    public void Dispose() {
        _colorsByName.Clear();
    }
}
