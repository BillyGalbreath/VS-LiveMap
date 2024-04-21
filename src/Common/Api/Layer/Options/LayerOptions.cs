using JetBrains.Annotations;
using LiveMap.Common.Api.Layer.Marker;
using Newtonsoft.Json;

namespace LiveMap.Common.Api.Layer.Options;

/// <summary>
/// A set of options for the base layer object of each marker<br/>
/// (<see cref="Polygon"/>, <see cref="Polyline"/>, <see cref="Circle"/>, etc.)
/// <remarks>
/// Do not use it directly
/// </remarks>
/// </summary>
[PublicAPI]
public abstract class LayerOptions : Options {
    /// <summary>
    /// When <see langword="true"/>, a mouse event on this marker will trigger the same event on the map
    /// </summary>
    [JsonProperty(Order = 0)]
    public bool? BubblingMouseEvents { get; set; }

    /// <summary>
    /// If <see langword="false"/>, this marker will not emit mouse events and will act as a part of the underlying map
    /// </summary>
    [JsonProperty(Order = 1)]
    public bool? Interactive { get; set; }

    /// <summary>
    /// Map pane where this marker will be added
    /// </summary>
    [JsonProperty(Order = 2)]
    public string? Pane { get; set; }

    /// <summary>
    /// String to be shown in the attribution control
    /// </summary>
    [JsonProperty(Order = 3)]
    public string? Attribution { get; set; }
}
