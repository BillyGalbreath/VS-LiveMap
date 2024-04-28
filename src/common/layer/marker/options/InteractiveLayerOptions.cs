using JetBrains.Annotations;
using Newtonsoft.Json;

namespace livemap.common.layer.marker.options;

/// <summary>
/// A set of options for an interactive overlay layer on the map<br/>
/// <remarks>
/// Do not use it directly
/// </remarks>
/// </summary>
[PublicAPI]
public abstract class InteractiveLayerOptions : LayerOptions {
    /// <summary>
    /// When <see langword="true"/>, a mouse event on this marker will trigger the same event on the map
    /// </summary>
    [JsonProperty(Order = 200)]
    public bool? BubblingMouseEvents { get; set; }

    /// <summary>
    /// If <see langword="false"/>, this marker will not emit mouse events and will act as a part of the underlying map
    /// </summary>
    [JsonProperty(Order = 201)]
    public bool? Interactive { get; set; }
}
