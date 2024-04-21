using JetBrains.Annotations;
using LiveMap.Common.Api.Layer.Marker;
using Newtonsoft.Json;

namespace LiveMap.Common.Api.Layer.Options.Marker;

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
