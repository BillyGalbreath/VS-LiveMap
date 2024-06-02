using Newtonsoft.Json;

namespace livemap.layer.marker.options;

/// <summary>
/// A set of options for an overlay layer on the map<br/>
/// <remarks>
/// Do not use it directly
/// </remarks>
/// </summary>
public class LayerOptions : BaseOptions {
    /// <summary>
    /// Map pane where this marker will be added
    /// </summary>
    [JsonProperty(Order = 300)]
    public string? Pane { get; set; }

    /// <summary>
    /// String to be shown in the attribution control
    /// </summary>
    [JsonProperty(Order = 301)]
    public string? Attribution { get; set; }
}
