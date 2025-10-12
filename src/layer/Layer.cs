using livemap.layer.marker;
using livemap.layer.marker.options;
using livemap.registry;
using livemap.util;
using Newtonsoft.Json;

namespace livemap.layer;

public abstract class Layer(string id, string label) : Keyed {
    [JsonProperty(Order = -10)] public string Id { get; } = id;

    [JsonProperty(Order = -9)] public string Label { get; } = label;

    [JsonProperty(Order = 0)] public virtual int? Interval { get; set; }

    [JsonProperty(Order = 1)] public virtual bool? Hidden { get; set; }

    [JsonProperty(Order = 10)] public virtual BaseOptions? Defaults { get; set; }

    [JsonProperty(Order = 11)] public virtual LayerOptions? Options { get; set; }

    [JsonProperty(Order = 999)] public virtual List<Marker> Markers { get; } = [];

    /// <summary>
    /// Custom CSS for this layer's map pane
    /// </summary>
    [JsonProperty(Order = 1000)]
    public virtual string? Css { get; set; }

    [JsonIgnore] public virtual string Filename => Path.Combine(Files.MarkerDir, $"{Id}.json");

    [JsonIgnore] public virtual bool Private { get; set; }

    public virtual async Task WriteToDisk(CancellationToken cancellationToken) {
        string layerJson = JsonConvert.SerializeObject(this, Files.JsonSerializerMinifiedSettings);

        if (cancellationToken.IsCancellationRequested) {
            return;
        }

        await Files.WriteJsonAsync(Filename, layerJson, cancellationToken);
    }
}
