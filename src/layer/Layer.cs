using System.Collections.Generic;
using JetBrains.Annotations;
using livemap.layer.marker;
using livemap.layer.marker.options;
using livemap.registry;
using Newtonsoft.Json;

namespace livemap.layer;

[PublicAPI]
public abstract class Layer : Keyed {
    [JsonProperty(Order = -10)]
    public string Id { get; }

    [JsonProperty(Order = -9)]
    public string Label { get; }

    [JsonProperty(Order = 0)]
    public int? Interval { get; set; }

    [JsonProperty(Order = 1)]
    public bool? Hidden { get; set; }

    [JsonProperty(Order = 10)]
    public BaseOptions? Defaults { get; set; }

    [JsonProperty(Order = 11)]
    public LayerOptions? Options { get; set; }

    [JsonProperty(Order = 999)]
    public List<Marker> Markers { get; } = new();

    protected Layer(string id, string label) {
        Id = id;
        Label = label;
    }
}
