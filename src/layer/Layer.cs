using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using livemap.layer.marker;
using livemap.layer.marker.options;
using livemap.registry;
using livemap.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace livemap.layer;

[PublicAPI]
public abstract class Layer : Keyed {
    [JsonProperty(Order = -10)]
    public string Id { get; }

    [JsonProperty(Order = -9)]
    public string Label { get; }

    [JsonProperty(Order = 0)]
    public virtual int? Interval { get; set; }

    [JsonProperty(Order = 1)]
    public virtual bool? Hidden { get; set; }

    [JsonProperty(Order = 10)]
    public virtual BaseOptions? Defaults { get; set; }

    [JsonProperty(Order = 11)]
    public virtual LayerOptions? Options { get; set; }

    [JsonProperty(Order = 999)]
    public virtual List<Marker> Markers { get; } = new();

    /// <summary>
    /// Custom CSS for this layer's map pane
    /// </summary>
    [JsonProperty(Order = 1000)]
    public virtual string? Css { get; set; }

    [JsonIgnore]
    public virtual string Filename => Path.Combine(Files.MarkerDir, $"{Id}.json");

    [JsonIgnore]
    public virtual bool Private { get; set; }

    protected Layer(string id, string label) {
        Id = id;
        Label = label;
    }

    public virtual async Task WriteToDisk(CancellationToken cancellationToken) {
        string layerJson = JsonConvert.SerializeObject(this, new JsonSerializerSettings {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        if (cancellationToken.IsCancellationRequested) {
            return;
        }

        await Files.WriteJsonAsync(Path.Combine(Files.MarkerDir, $"{Id}.json"), layerJson, cancellationToken);
    }
}
