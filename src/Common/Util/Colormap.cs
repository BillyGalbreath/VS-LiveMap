using System.Collections.Generic;
using Vintagestory.API.Util;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LiveMap.Common.Util;

public sealed class Colormap {
    [YamlMember] private readonly Dictionary<string, int[]> _colors = new();

    public void Add(string block, int[] toAdd) {
        _colors.TryAdd(block, toAdd);
    }

    public int[]? Get(string block) {
        return _colors!.Get(block);
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
        Dictionary<string, int[]> data = new DeserializerBuilder().IgnoreUnmatchedProperties()
            .WithNamingConvention(NullNamingConvention.Instance)
            .Build()
            .Deserialize<Dictionary<string, int[]>>(yaml);

        Colormap colormap = new();
        foreach ((string? key, int[]? value) in data) {
            colormap._colors.TryAdd(key, value);
        }

        return colormap;
    }

    public void Dispose() {
        _colors.Clear();
    }
}
