using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using livemap.configuration;
using livemap.layer.marker;
using livemap.util;
using Vintagestory.API.Util;

namespace livemap.layer.builtin;

public class TranslocatorsLayer : Layer {
    public override int? Interval => Config.UpdateInterval;

    public override bool? Hidden => !Config.DefaultShowLayer;

    public override List<Marker> Markers {
        get {
            List<Marker> list = new();
            _knownTranslocators.Values.Foreach(translocators => {
                translocators.Foreach(translocator => {
                    // todo
                });
            });
            return list;
        }
    }

    public override string Filename => Path.Combine(Files.MarkerDir, $"{Id}.json");

    private static Translocators Config => LiveMap.Api.Config.Layers.Translocators;

    private readonly ConcurrentDictionary<ulong, HashSet<Translocator>> _knownTranslocators = new();

    public TranslocatorsLayer() : base("translocators", "lang.translocators".ToLang()) { }

    public void SetTranslocators(ulong chunkIndex, HashSet<Translocator> translocator) {
        _knownTranslocators[chunkIndex] = translocator;
    }

    public class Translocator {
        //
    }
}
