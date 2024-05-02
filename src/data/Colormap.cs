using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using livemap.logger;
using livemap.network.packet;
using livemap.util;
using Vintagestory.API.Common;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace livemap.data;

[PublicAPI]
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

    public int Count => _colorsById.Count;

    public string Serialize() {
        return new SerializerBuilder()
            .WithQuotingNecessaryStrings()
            .WithNamingConvention(NullNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build()
            .Serialize(_colorsByName);
    }

    public bool Deserialize(string? yaml) {
        _colorsByName.Clear();

        if (string.IsNullOrEmpty(yaml)) {
            return false;
        }

        try {
            Dictionary<string, uint[]> data = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(NullNamingConvention.Instance)
                .Build().Deserialize<Dictionary<string, uint[]>>(yaml);
            foreach ((string? key, uint[]? colors) in data) {
                _colorsByName.TryAdd(key, colors);
            }
            return true;
        } catch (Exception e) {
            Logger.Error(e.ToString());
            return false;
        }
    }

    public void LoadFromPacket(IWorldAccessor world, ColormapPacket packet) {
        new Thread(_ => {
            if (Deserialize(packet.RawColormap)) {
                SaveToDisk();
                RefreshIds(world);
                Logger.Info("&dColormap saved to disk.");
            } else {
                Logger.Warn("Could not save colormap to disk.");
            }
        }).Start();
    }

    public void LoadFromDisk(IWorldAccessor world) {
        new Thread(_ => {
            string? yaml = null;
            if (File.Exists(Files.ColormapFile)) {
                yaml = File.ReadAllText(Files.ColormapFile, Encoding.UTF8);
            }
            if (Deserialize(yaml)) {
                RefreshIds(world);
                Logger.Info("&dColormap loaded from disk.");
            } else {
                Logger.Warn("Could not load colormap from disk.");
                Logger.Warn("An admin needs to send the colormap from their client.");
            }
        }).Start();
    }

    public void SaveToDisk() {
        File.WriteAllText(Files.ColormapFile, Serialize(), Encoding.UTF8);
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
