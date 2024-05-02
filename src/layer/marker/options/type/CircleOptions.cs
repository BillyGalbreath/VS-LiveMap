using JetBrains.Annotations;
using Newtonsoft.Json;

namespace livemap.layer.marker.options.type;

/// <summary>
/// Optional settings for the <see cref="Circle"/> marker
/// </summary>
[PublicAPI]
public class CircleOptions : PathOptions {
    /// <summary>
    /// Radius of the circle, in blocks
    /// </summary>
    [JsonProperty(Order = 0)]
    public double? Radius { get; set; }
}
