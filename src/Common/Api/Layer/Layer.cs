using JetBrains.Annotations;
using LiveMap.Common.Api.Layer.Marker;
using LiveMap.Common.Api.Layer.Options;
using Newtonsoft.Json;

namespace LiveMap.Common.Api.Layer;

[PublicAPI]
public class Layer {
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
    public AbstractMarker[]? Markers { get; set; }

    public Layer(string id, string label) {
        Id = id;
        Label = label;
    }
}
