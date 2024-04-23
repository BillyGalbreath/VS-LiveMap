using JetBrains.Annotations;
using livemap.common.api.layer.marker;
using Newtonsoft.Json;

namespace livemap.common.api.layer.options.marker;

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
