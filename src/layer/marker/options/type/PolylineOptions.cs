using JetBrains.Annotations;
using Newtonsoft.Json;

namespace livemap.layer.marker.options.type;

/// <summary>
/// Optional settings for the <see cref="Polyline"/> marker
/// </summary>
[PublicAPI]
public class PolylineOptions : PathOptions {
    /// <summary>
    /// How much to simplify the polyline on each zoom level. More means better performance and smoother look, and fewer means more accurate representation
    /// </summary>
    [JsonProperty(Order = 0)]
    public double? SmoothFactor { get; set; }

    /// <summary>
    /// Disable polyline clipping
    /// </summary>
    [JsonProperty(Order = 1)]
    public bool? NoClip { get; set; }
}
