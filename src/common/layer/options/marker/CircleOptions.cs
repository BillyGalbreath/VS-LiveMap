using JetBrains.Annotations;
using livemap.common.layer.marker;
using Newtonsoft.Json;

namespace livemap.common.layer.options.marker;

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
